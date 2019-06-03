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

        public GoldBalancePage() {
            this.InitializeComponent();
            this.mainWindow = Application.Current.Windows.OfType<MainWindow>().ElementAt(0);
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

            string s = this.ItemsView.SelectedItem as string;
            if (s == null) return null;
            Decimal.TryParse(this.PriceTextBox.Text, out decimal price);    
            UInt64.TryParse(this.AmountTextBox.Text, out ulong amount);

            return new Item(s, amount, price);
        }

        public DailyGoldBalance AddDaily() {
            string date = DateUtils.GetTodaysDate();
            foreach (var entry in this.GoldBalanceView.Items) {
                if (entry is DailyGoldBalance e) {
                    if (e.Date == date) {
                        return e;
                    }
                }
            }
            var newDaily = new DailyGoldBalance();
            List<DailyGoldBalance> tempList = new List<DailyGoldBalance>(this.mainWindow.DailyGoldBalances) { newDaily };
            tempList = tempList.OrderByDescending(s => s.Date).ToList();
            this.mainWindow.DailyGoldBalances = new System.Collections.ObjectModel.ObservableCollection<DailyGoldBalance>(tempList);
            this.GoldBalanceView.ItemsSource = this.mainWindow.DailyGoldBalances;
            this.UpdateTreeView();
            return newDaily;
        }

        public void AddItem() {
            Item item = this.CreateItem();
            if (item == null) {
                MessageBox.Show("Error! Can't add a new item. Check if the values provided are correct.", "AddError", MessageBoxButton.OK);
                return;
            }

            ItemType soldOrBought = (ItemType)Enum.Parse(typeof(ItemType), this.ItemTypeComboBox.SelectedItem as string);

            object o = this.GoldBalanceView.SelectedItem;
            if (o == null) {
                var daily = this.AddDaily();
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
                    ex?.Add(item);
                } else if (o is DailyEarnings er) {
                    if (soldOrBought == ItemType.Bought) {
                        MessageBox.Show("You are trying to add Bought item to the Earnings!");
                        return;
                    }
                    er?.Add(item);
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
            this.UpdateTreeView();
            this.AmountTextBox.Text = "";
            this.PriceTextBox.Text = "";
        }

        public void DeleteItem() {
            object o = this.GoldBalanceView.SelectedItem;
            if (o == null) return;
            if (o is DailyGoldBalance gb) {
                MessageBoxResult res = MessageBox.Show("Do you really want to delete this item?", "Delete", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes) {
                    this.mainWindow.DailyGoldBalances.Remove(gb);
                }
            } else if (o is Item i) {
                var owner = i.GetOwner();
                if (owner == null) return;
                if (owner is DailyExpenses ex) {
                    MessageBoxResult res = MessageBox.Show("Do you really want to delete this item?", "Delete", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes) {
                        ex.Remove(i);
                    }
                } else if (owner is DailyEarnings er) {
                    MessageBoxResult res = MessageBox.Show("Do you really want to delete this item?", "Delete", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes) {
                        er.Remove(i);
                    }
                }
            }
        }

        public void PopulateViews() {
            this.ItemTypeComboBox.ItemsSource = Enum.GetNames(typeof(ItemType));
            this.ItemTypeComboBox.SelectedItem = this.ItemTypeComboBox.Items.GetItemAt(0);
            this.ItemsView.ItemsSource = Earnings.ItemNames;
            this.GoldBalanceView.ItemsSource = this.mainWindow.DailyGoldBalances;
            this.UpdateTreeView();
        }

        private void UpdateTreeView() {
            this.GoldBalanceView.UpdateLayout();
            this.GoldBalanceView.Items.Refresh();
        }

        public void DrawChart() {
            try {
                var t = Application.Current.Windows.OfType<BalancePlot>().ElementAt(0);
            } catch (ArgumentOutOfRangeException) {
                (new BalancePlot()).Show();
            }
        }

        private void NumericValidation(object sender, TextCompositionEventArgs e) => e.Handled = !StringUtils.IsNumeric(e.Text);

        private void AddDailyEvent(object sender, RoutedEventArgs e) => this.AddDaily();

        private void AddItemEvent(object sender, RoutedEventArgs e) => this.AddItem();

        private void DeleteItemButton_Click(object sender, RoutedEventArgs e) => this.DeleteItem();

        private void GoldBalanceView_Selected(object sender, RoutedEventArgs e) {
            this.InfoBox.Text = this.GoldBalanceView.SelectedItem?.ToString();
        }

        private void AddToItemDatabaseEvent(object sender, RoutedEventArgs e) {
            if (String.IsNullOrWhiteSpace(this.FindItemsTextBox.Text)) return;
            Earnings.ItemNames.Add(this.FindItemsTextBox.Text.Trim().CapitalizeSentenceWords());
            this.ItemsView.UpdateLayout();
            this.FindItemsTextBox.Text = "";
        }

        private void FindItemsOnTextChangedEvent(object sender, TextChangedEventArgs e) {
            this.ItemsView.ItemsSource = Earnings.ItemNames.Where(s => s.ToLower().Contains(this.FindItemsTextBox.Text.ToLower().Trim())).OrderBy(s => s);
        }


    }
}
