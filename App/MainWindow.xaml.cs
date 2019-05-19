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
using System.Windows.Input;

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

        private const string copiedFilesFilePath = @"../../Copies";

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
            this.UpdateOwners();
            //ALWAYS BACK UP YOUR WORK KIDDOS
            this.MakeFilesBackup();
            GC.Collect();
        }

        private void MakeFilesBackup() {
            string jsonFilesPath = copiedFilesFilePath + "/" + @"JsonFiles_Copies";
            string tasksPath = copiedFilesFilePath + "/" + @"Tasks_Copies";
            string moneyBalancesPath = copiedFilesFilePath + "/" + @"MoneyBalances_Copies";
            string[] jsonFiles, tasksFiles, moneyBalanceFiles;
            jsonFiles = Directory.GetFiles(@"../../JsonFiles");
            tasksFiles = Directory.GetFiles(@"../../Tasks");
            moneyBalanceFiles = Directory.GetFiles(@"../../MoneyBalances");

            foreach (var jsonFile in jsonFiles) {
                string filename = jsonFilesPath + "/" + Path.GetFileNameWithoutExtension(jsonFile) + " Copy" + Path.GetExtension(jsonFile);
                File.Copy(jsonFile, filename, true);
            }

            foreach (var taskFile in tasksFiles) {
                string filename = tasksPath + "/" + Path.GetFileNameWithoutExtension(taskFile) + " Copy" + Path.GetExtension(taskFile);
                File.Copy(taskFile, filename, true);
            }

            foreach (var moneyBalanceFile in moneyBalanceFiles) {
                string filename = moneyBalancesPath + "/" + Path.GetFileNameWithoutExtension(moneyBalanceFile) + " Copy" + Path.GetExtension(moneyBalanceFile);
                File.Copy(moneyBalanceFile, filename, true);
            }
        }

        private void UpdateOwners() {
            foreach (var balance in this.DailyGoldBalances) {
                balance.UpdateOwners();
            }
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
                this.DailySlayerTasks.Add(l);
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
                this.DailyGoldBalances.Add(gb);
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
            this.slayerPage = new SlayerPage();
            f1.Content = slayerPage;
            this.bossLogsPage = new BossLogsPage();
            f3.Content = bossLogsPage;
            this.goldBalancePage = new GoldBalancePage();
            f2.Content = goldBalancePage;
            item1.Content = f1;
            item2.Content = f2;
            item3.Content = f3;
            this.Tabs.Items.Add(item1);
            this.Tabs.Items.Add(item2);
            this.Tabs.Items.Add(item3);
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
            this.FinishedSaving = false;
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

        public void DrawChart() {
            this.Dispatcher.Invoke(() => {
                if (this.Tabs.SelectedItem == null) return;
                if (this.Tabs.SelectedItem is TabItem ti) {
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

        //event handlers
        private void OnWindowClose(object sender, System.ComponentModel.CancelEventArgs e) {
            this.slayerPage.KillAndClearChartProcess();
            this.goldBalancePage.KillAndClearChartProcess();
            MessageBoxResult savingFlag = MessageBox.Show("Do you want to save unsaved changes?", "Save changes", MessageBoxButton.YesNo);
            if (savingFlag == MessageBoxResult.Yes) {
                this.SaveAllProgress();
            }
            foreach (var window in Application.Current.Windows) {
                try {
                    ((Window)window).Close();
                } catch (Exception) {}
            }
        }

        public void SaveProgress() {
            if (!this.FinishedSaving) return;
            (new Thread(() => this.SaveAllProgress())).Start();
        }

        private void SaveAllProgressEvent(object sender, RoutedEventArgs e) => this.SaveProgress();

        private void DrawChartMainWindowEvent(object sender, RoutedEventArgs e) => (new Thread(() => this.DrawChart())).Start();

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.S)) {
                this.SaveProgress();
            }
        }
    }
}
