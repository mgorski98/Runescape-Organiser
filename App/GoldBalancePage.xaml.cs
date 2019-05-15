#undef DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Utils;

namespace RunescapeOrganiser {
    /// <summary>
    /// Interaction logic for GoldBalancePage.xaml
    /// </summary>
    public partial class GoldBalancePage : Page {

        public MainWindow mainWindow;
        public bool processExited = true;
        private Process chartProcess = null;

        public GoldBalancePage() {
            InitializeComponent();
            mainWindow = Application.Current.Windows.OfType<MainWindow>().ElementAt(0);
            #region TREEVIEW_TEST
#if DEBUG
            DailyGoldBalance balance = new DailyGoldBalance();
            DailyExpenses expense = new DailyExpenses();
            DailyEarnings earning = new DailyEarnings();
            expense.BoughtItems.Add(new Item("Arrow Shaft", 20, 24));
            earning.Add(new Item("Arrow Shaft", 40, 80));
            balance.EarningsAndExpenses.Add(earning);
            balance.EarningsAndExpenses.Add(expense);
            mainWindow.DailyGoldBalances.Add(balance);
            this.GoldBalanceView.ItemsSource = mainWindow.DailyGoldBalances;
#endif
            #endregion 
        }

        public Item CreateItem() {

            string s = ItemsView.SelectedItem as string;
            if (s == null) return null;
            Decimal.TryParse(PriceTextBox.Text, out decimal price);
            UInt64.TryParse(AmountTextBox.Text, out ulong amount);

            return new Item(s, amount, price);
        }

        public DailyGoldBalance AddDaily() {

            string date = DateUtils.GetTodaysDate();
            foreach (var entry in GoldBalanceView.Items) {
                if (entry is DailyGoldBalance e) {
                    if (e.Date == date) {
                        return e;
                    }
                }
            }
            var newDaily = new DailyGoldBalance();
            List<DailyGoldBalance> tempList = new List<DailyGoldBalance>(mainWindow.DailyGoldBalances) { newDaily };
            tempList = tempList.OrderByDescending(s => s.Date).ToList();
            mainWindow.DailyGoldBalances = new System.Collections.ObjectModel.ObservableCollection<DailyGoldBalance>(tempList);
            this.GoldBalanceView.ItemsSource = mainWindow.DailyGoldBalances;
            UpdateTreeView();
            return newDaily;
        }

        public void AddItem() {
            Item item = CreateItem();
            if (item == null) {
                MessageBox.Show("Error! Can't add a new item. Check if the values provided are correct.", "AddError", MessageBoxButton.OK);
                return;
            }

            ItemType soldOrBought = (ItemType)Enum.Parse(typeof(ItemType), ItemTypeComboBox.SelectedItem as string);

            object o = GoldBalanceView.SelectedItem;
            if (o == null) {
                var daily = AddDaily();
                switch (soldOrBought) {
                    case ItemType.Bought:
                        (daily.EarningsAndExpenses[1] as DailyExpenses)?.Add(item);
                        break;
                    case ItemType.Sold:
                        (daily.EarningsAndExpenses[0] as DailyEarnings)?.Add(item);
                        break;
                }
            } else {
                if (o is DailyGoldBalance gb) {
                    switch (soldOrBought) {
                        case ItemType.Bought:
                            (gb.EarningsAndExpenses[1] as DailyExpenses)?.Add(item);
                            break;
                        case ItemType.Sold:
                            (gb.EarningsAndExpenses[0] as DailyEarnings)?.Add(item);
                            break;
                    }
                } else if (o is DailyExpenses ex) {
                    if (soldOrBought == ItemType.Sold) {
                        MessageBox.Show("You are trying to add Sold item to the Expenses!");
                        return;
                    }
                    ex.BoughtItems.Add(item);
                } else if (o is DailyEarnings er) {
                    if (soldOrBought == ItemType.Bought) {
                        MessageBox.Show("You are trying to add Bought item to the Earnings!");
                        return;
                    }
                    er.SoldItems.Add(item);
                } else if (o is Item i) {
                    DailyGoldBalance _gb = i.GetOwner()?.GetOwner();
                    switch (soldOrBought) {
                        case ItemType.Bought:
                            (_gb?.EarningsAndExpenses[1] as DailyExpenses)?.Add(item);
                            break;
                        case ItemType.Sold:
                            (_gb?.EarningsAndExpenses[0] as DailyEarnings)?.Add(item);
                            break;
                    }
                }
            }
            UpdateTreeView();
        }

        public void DeleteItem() {

        }

        public void PopulateViews() {
            this.ItemTypeComboBox.ItemsSource = Enum.GetNames(typeof(ItemType));
            this.ItemTypeComboBox.SelectedItem = this.ItemTypeComboBox.Items.GetItemAt(0);
            this.ItemsView.ItemsSource = Earnings.ItemNames;
            this.GoldBalanceView.ItemsSource = mainWindow.DailyGoldBalances;
            this.UpdateTreeView();
        }

        private void UpdateTreeView() {
            this.GoldBalanceView.UpdateLayout();
            this.GoldBalanceView.Items.Refresh();
        }

        public void DrawChart() {
            if (this.processExited == false) return;
            var chartData = GoldBalanceView.Items.Cast<DailyGoldBalance>();
            if (chartData.Count() <= 0) {
                MessageBox.Show("Not enough data to draw a chart!", "ChartError", MessageBoxButton.OK);
                return;
            }
            StringBuilder args = new StringBuilder();
            chartProcess = new Process();

            args.Append(chartData.Count().ToString() + ' ');
            foreach (var elem in chartData) {
                args.Append(elem.Date + ' ');
                args.Append(elem.GetEarnings().ToString() + ' ');
                args.Append(elem.GetExpenses().ToString() + ' ');
            }
            chartProcess.StartInfo.FileName = @"..\..\PythonScripts\BalancePlot.py";
            chartProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            chartProcess.StartInfo.Arguments = args.ToString();

            try {
                chartProcess.Start();
                processExited = false;
            } catch (Exception) {
                MessageBox.Show("Error: Cannot find a file BalancePlot.py");
                return;
            }

            chartProcess.WaitForExit();
            chartProcess?.Dispose();
            processExited = true;
        }

        public void KillAndClearChartProcess() {
            try {
                mainWindow = null;
                chartProcess?.Kill();
                chartProcess?.Dispose();
                chartProcess = null;
            } catch (Exception) { }
        }

        private void AmountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = !StringUtils.IsNumeric(e.Text);
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = !StringUtils.IsNumeric(e.Text);
        }

        private void AddDailyButton_Click(object sender, RoutedEventArgs e) {
            AddDaily();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {//add item
            AddItem();
        }

        private void GoldBalanceView_Selected(object sender, RoutedEventArgs e) {
            InfoBox.Text = GoldBalanceView.SelectedItem?.ToString();
        }
    }
}
