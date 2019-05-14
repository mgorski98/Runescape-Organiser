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
using System.Threading;
using Utils;


namespace RunescapeOrganiser {
    /// <summary>
    /// Interaction logic for EarningsPage.xaml
    /// </summary>
    public partial class EarningsPage : Page {

        public MainWindow mainWindow = null;
        public bool processExited = true;
        private Process chartProcess = null;

        public EarningsPage() {
            InitializeComponent();
            mainWindow = Application.Current.Windows.OfType<MainWindow>().ElementAt(0);
        }

        public void InitItemsView() {
            this.ItemsView.ItemsSource = Earnings.ItemNames;
            this.ItemsView.UpdateLayout();
            this.ItemsView.Items.Refresh();
        }

        public DailyEarnings AddDaily() {
            string date = DateUtils.GetTodaysDate();
            foreach (var entry in EarningsView.Items) {
                if (entry is DailyEarnings e) {
                    if (e.Date == date) {
                        return e;
                    }
                }
            }
            var newDaily = new DailyEarnings();
            List<DailyEarnings> tempList = new List<DailyEarnings>(mainWindow.@DailyEarnings) { newDaily };
            tempList = tempList.OrderByDescending(s => s?.Date).ToList();
            mainWindow.DailyEarnings = new System.Collections.ObjectModel.ObservableCollection<DailyEarnings>(tempList);
            EarningsView.ItemsSource = mainWindow.DailyEarnings;
            UpdateEarningsView();
            return newDaily;
        }

        public void UpdateEarningsView() {
            EarningsView.UpdateLayout();
            EarningsView.Items.Refresh();
        }

        public void DeleteItem() {
            object o = EarningsView.SelectedItem;
            if (o == null) return;
            if (o is DailyEarnings e) {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the daily?", "Delete Daily", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes) {
                    mainWindow.DailyEarnings.Remove(e);
                    UpdateEarningsView();
                    ItemInfo.Text = "";
                }
            }
            if (o is Item item) {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Delete", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes) {
                    item.GetOwner().Remove(item);
                    UpdateEarningsView();
                    ItemInfo.Text = "";
                }
            }
        }

        public void DrawChart() {
            if (processExited == false) return;
            var chartData = EarningsView.Items.Cast<DailyEarnings>();
            if (chartData.Count() <= 1) {
                MessageBox.Show("Not enough data to draw a chart!", "ChartError", MessageBoxButton.OK);
                return;
            }
            chartProcess = new Process();
            StringBuilder args = new StringBuilder();
            args.Append(chartData.Count().ToString() + ' ');
            foreach (var element in chartData) {
                args.Append(element.Date + ' ');
            }
            args.Append(chartData.Count().ToString() + ' ');
            foreach (var element in chartData) {
                args.Append(element.TotalMoneyEarned().ToString() + ' ');
            }
            chartProcess.StartInfo.Arguments = args.ToString();
            chartProcess.StartInfo.FileName = @"..\..\PythonScripts\EarningsPlot.py";
            chartProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            try {
                chartProcess.Start();
                processExited = false;
            } catch (Exception) {
                MessageBox.Show("Error: Cannot find a file EarningsPlot.py");
                return;
            }
            chartProcess.WaitForExit();
            chartProcess?.Dispose();
            processExited = true;
        }

        public Item CreateSoldItem() {
            Item item = null;

            string itemName = ItemsView.SelectedItem as string;
            if (itemName == null) return null;

            Decimal.TryParse(PriceTextBox.Text, out decimal price);
            UInt64.TryParse(AmountTextBox.Text, out ulong amount);

            item = new Item(itemName, amount, price);

            return item;
        }

        public void AddSoldItem() {
            Item item = CreateSoldItem();
            if (item == null) {
                MessageBox.Show("Error! Can't add a new item. Check if the values provided are correct.", "AddError", MessageBoxButton.OK);
                return;
            }
            DailyEarnings parent = AddDaily();
            parent.Add(item);
            UpdateEarningsView();
        }

        public void KillAndClearChartProcess() {
            try {
                mainWindow = null;
                chartProcess?.Kill();
                chartProcess?.Dispose();
                chartProcess = null;
            } catch (Exception) { }
        }

        private void EarningsView_Selected(object sender, RoutedEventArgs e) {
            ItemInfo.Text = EarningsView.SelectedItem?.ToString();
        }

        private void AddDailyButton_Click(object sender, RoutedEventArgs e) {
            AddDaily();
        }

        private void DeleteItemButton_Click(object sender, RoutedEventArgs e) {
            DeleteItem();
        }

        private void ShowGraphButton_Click(object sender, RoutedEventArgs e) {
            Thread t = new Thread(() => DrawChart());
            t.Start();
        }

        private void FindItemsTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            this.ItemsView.ItemsSource = Earnings.ItemNames.Where(s => s.ToLower().Contains(FindItemsTextBox.Text.ToLower())).OrderBy(s => s);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (String.IsNullOrWhiteSpace(FindItemsTextBox.Text)) return;
            Earnings.ItemNames.Add(FindItemsTextBox.Text.Trim().Capitalize());
            this.ItemsView.UpdateLayout();
            this.FindItemsTextBox.Text = "";
        }

        private void AmountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = !StringUtils.IsNumeric(e.Text);
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = !StringUtils.IsNumeric(e.Text);
        }

        private void AddSoldItemEvent(object sender, RoutedEventArgs e) {
            AddSoldItem();
            UpdateEarningsView();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) {
            mainWindow.SaveProgress();
        }
    }
}
