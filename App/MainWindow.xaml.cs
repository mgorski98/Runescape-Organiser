using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

//TODO: Add controls to differentiate chosen item type (is it bought (expense) or sold (earning))
//TODO: Add ObservableCollection<DailyMoneyBalance> to MainWindow Class
//TODO: Add loading daily balance from json
//TODO: Set up owners in Daily Earnings and Daily Expenses
//TODO: create a DailyMoneyBalance class to store Daily Earnings and Daily Expenses
//TODO: set up a treeview suited for Daily money balance (money balance -> earnings and expenses -> items)
//TODO: delete earnings page and expenses page after the combined work
//TODO: modify python script responsible for drawing earnings chart to include expenses on the same plot (also change the way the arguments are passed)
namespace RunescapeOrganiser {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public ObservableCollection<DailySlayerTaskList> DailySlayerTasks {
            get;set;
        }

        public ObservableCollection<DailyGoldBalance> DailyGoldBalances {
            get;set;
        }

        private static object taskThreadLock = new object();
        private static object balanceThreadLock = new object();

        private bool FinishedLoadingTasks {
            get; set;
        }

        private bool FinishedLoadingBalances {
            get;set;
        }

        private bool FinishedSaving {
            get;set;
        }

        private SlayerPage slayerPage;
        private BossLogsPage bossLogsPage;
        private GoldBalancePage goldBalancePage;

        private List<DailyGoldBalance> balances = new List<DailyGoldBalance>();

        public MainWindow() {
            //init collections
            this.DailyGoldBalances = new ObservableCollection<DailyGoldBalance>();
            this.FinishedSaving = true;
            this.DailySlayerTasks = new ObservableCollection<DailySlayerTaskList>();
            //init windows
            this.InitializeComponent();
            this.ApplicationInit();
            //init databases
            Slayer.InitSlayerTables();
            Earnings.InitItemNames();
            //load collection items
            this.LoadTasks();
            this.LoadBalances();
            //init the views
            this.goldBalancePage.PopulateViews();
            GC.Collect();
        }

        private void LoadTasks() {
            string[] files = Directory.GetFiles(@"../../Tasks");
            List<Thread> threads = new List<Thread>();
            foreach (var file in files) {
                Thread t = new Thread(() => LoadTaskFromJson(file));
                threads.Add(t);
                t.Start();
            }
            while (!this.FinishedLoadingTasks) {
                this.FinishedLoadingTasks = threads.All(t => !t.IsAlive);
            }
            List<DailySlayerTaskList> tempList = new List<DailySlayerTaskList>(this.DailySlayerTasks);
            tempList = tempList.OrderByDescending(task => task.TaskDate).ToList();
            this.DailySlayerTasks = new ObservableCollection<DailySlayerTaskList>(tempList);
            this.Dispatcher.Invoke(() => slayerPage.SlayerTasksView.ItemsSource = this.DailySlayerTasks);
        }

        private void LoadTaskFromJson(string path) {
            if (String.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;

            DailySlayerTaskList l = null;

            using (var reader = new StreamReader(path)) {
                l = JsonConvert.DeserializeObject<DailySlayerTaskList>(reader.ReadToEnd());
            }

            lock (taskThreadLock) {
                DailySlayerTasks.Add(l);
            }
        }

        private void LoadBalances() {
            string[] files = Directory.GetFiles(@"../../MoneyBalances");
            List<Thread> threads = new List<Thread>();
            foreach (var file in files) {
                Thread t = new Thread(() => LoadBalanceFromJson(file));
                threads.Add(t);
                t.Start();
            }

            while (!this.FinishedLoadingBalances) {
                this.FinishedLoadingBalances = threads.All(t => !t.IsAlive);
            }
            List<DailyGoldBalance> tempList = new List<DailyGoldBalance>(this.DailyGoldBalances);
            tempList = tempList.OrderByDescending(balance => balance.Date).ToList();
            this.DailyGoldBalances = new ObservableCollection<DailyGoldBalance>(tempList);
            this.Dispatcher.Invoke(() => goldBalancePage.GoldBalanceView.ItemsSource = this.DailyGoldBalances);
        }

        private void LoadBalanceFromJson(string path) {
            if (String.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;
            DailyGoldBalance gb = null;

            using (var reader = new StreamReader(path)) {
                gb = JsonConvert.DeserializeObject<DailyGoldBalance>(reader.ReadToEnd());
            }
            lock (balanceThreadLock) {
                DailyGoldBalances.Add(gb);
            }
        }

        private void ApplicationInit() {
            TabItem item1, item2, item3;
            item1 = new TabItem();
            item2 = new TabItem();
            item3 = new TabItem();
            Frame f1, f2, f3;
            f1 = new Frame();
            f2 = new Frame();
            f3 = new Frame();
            f1.Background = Brushes.LightGray;
            f2.Background = Brushes.LightGray;
            f3.Background = Brushes.LightGray;
            item1.Header = "Slayer tasks";
            item2.Header = "Gold balances";
            item3.Header = "Boss logs";
            slayerPage = new SlayerPage();
            f1.Content = slayerPage;
            bossLogsPage = new BossLogsPage();
            f3.Content = bossLogsPage;
            goldBalancePage = new GoldBalancePage();
            f2.Content = goldBalancePage;
            item1.Content = f1;
            item2.Content = f2;
            item3.Content = f3;
            Tabs.Items.Add(item1);
            Tabs.Items.Add(item2);
            Tabs.Items.Add(item3);
        }

        public void ShowAddTaskWindow() {
            try {
                var t = Application.Current.Windows.OfType<TaskAddWindow>().ElementAt(0);
            } catch (ArgumentOutOfRangeException) {
                var taskadd = new TaskAddWindow();
                taskadd.Show();
                taskadd.BossListView.Items.Refresh();
                taskadd.HideBossControls();
            }
        }

        private void SaveAllProgress() {
            List<Thread> threads = new List<Thread>();
            Thread items = new Thread(() => Earnings.DumpToDisk());
            threads.Add(items);
            items.Start();
            foreach (var balance in DailyGoldBalances) {
                Thread t = new Thread(() => balance.SaveToJson());
                threads.Add(t);
                t.Start();
            }

            while (!this.FinishedSaving) {
                this.FinishedSaving = threads.All(t => !t.IsAlive);
            }

            this.FinishedSaving = true;
            MessageBox.Show("Progress saved successfully!", "Saving", MessageBoxButton.OK);
        }


        //event handlers
        private void OnWindowClose(object sender, System.ComponentModel.CancelEventArgs e) {
            slayerPage.KillAndClearChartProcess();
            goldBalancePage.KillAndClearChartProcess();
            Earnings.DumpToDisk();
        }

        public void SaveProgress() {
            if (!this.FinishedSaving) return;
            Thread t = new Thread(() => this.SaveAllProgress());
            t.Start();
        }

        public void DrawChart() {
            this.Dispatcher.Invoke(() => {
                if (this.Tabs.SelectedItem == null) return;
                if (Tabs.SelectedItem is TabItem ti) {
                    if (ti.Content is Frame f) {
                        if (f.Content is SlayerPage p1) {
                            (new Thread(() => p1.DrawChart())).Start();
                        } else if (f.Content is GoldBalancePage p2) {
                            (new Thread(() => p2.DrawChart())).Start();
                        }
                    }
                }
            });
        }

        private void SaveAllProgressEvent(object sender, RoutedEventArgs e) {
            this.SaveProgress();
        }

        private void DrawChartMainWindowEvent(object sender, RoutedEventArgs e) {
            (new Thread(() => this.DrawChart())).Start();
        }
    }
}
