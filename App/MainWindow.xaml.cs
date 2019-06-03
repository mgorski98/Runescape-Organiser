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
using System.Threading.Tasks;

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

        public ObservableCollection<DailyTimeStatistic> DailyTimeStatistics {
            get; set;
        }

        private static object taskThreadLock = new object();
        private static object balanceThreadLock = new object();
        private static object statisticThreadLock = new object();

        private const string copiedFilesFilePath = @"../../Copies";

        private bool FinishedSaving {
            get;set;
        }

        private SlayerPage slayerPage;
        private GoldBalancePage goldBalancePage;
        private InGameTimePage inGameTimePage;

        public MainWindow() {
            //init collections
            this.FinishedSaving = true;
            this.DailyGoldBalances = new ObservableCollection<DailyGoldBalance>();
            this.DailySlayerTasks = new ObservableCollection<DailySlayerTaskList>();
            this.DailyTimeStatistics = new ObservableCollection<DailyTimeStatistic>();
            //init windows
            this.InitializeComponent();
            this.ApplicationInit();
            //init databases
            Slayer.InitSlayerTables();
            Earnings.InitItemNames();
            //load collection items
            this.LoadTasks();
            this.LoadBalances();
            this.LoadTimeStatistics();
            this.InitTimeStatistics();
            //init the views
            this.goldBalancePage.PopulateViews();
            //ALWAYS BACK UP YOUR WORK KIDDOS
            this.MakeFilesBackup();
            GC.Collect();
            this.UpdateOwners();
            //this.inGameTimePage.DrawChart();
        }

        private void InitTimeStatistics() {
            foreach (var stat in this.DailyTimeStatistics) {
                stat.Init();
            }
        }

        public SlayerPage GetSlayerPage() => this.slayerPage;
        public InGameTimePage GetInGameTimePage() => this.inGameTimePage;
        public GoldBalancePage GetGoldBalancePage() => this.goldBalancePage;

        /// <summary>
        /// makes a backup of task, balance and data files
        /// </summary>
        private void MakeFilesBackup() {
            string jsonFilesPath = copiedFilesFilePath + "/" + @"JsonFiles_Copies";
            string tasksPath = copiedFilesFilePath + "/" + @"Tasks_Copies";
            string moneyBalancesPath = copiedFilesFilePath + "/" + @"MoneyBalances_Copies";
            string timeStatisticsPath = copiedFilesFilePath + "/" + @"TimeStatistics_Copies";
            string[] jsonFiles, tasksFiles, moneyBalanceFiles, timeStatisticsFiles;
            jsonFiles = Directory.GetFiles(@"../../JsonFiles");
            tasksFiles = Directory.GetFiles(@"../../Tasks");
            moneyBalanceFiles = Directory.GetFiles(@"../../MoneyBalances");
            timeStatisticsFiles = Directory.GetFiles(@"../../TimeStatistics");

            void CopyFiles(string file, string basePath) {
                string filename = basePath + "/" + Path.GetFileNameWithoutExtension(file) + " Copy" + Path.GetExtension(file);
                File.Copy(file, filename, true);
            }

            Parallel.ForEach(jsonFiles, elem => CopyFiles(elem, jsonFilesPath));
            Parallel.ForEach(tasksFiles, elem => CopyFiles(elem, tasksPath));
            Parallel.ForEach(moneyBalanceFiles, elem => CopyFiles(elem, moneyBalancesPath));
            Parallel.ForEach(timeStatisticsFiles, elem => CopyFiles(elem, timeStatisticsPath));
        }

        /// <summary>
        /// updates the owners of each tree item (owners are the same as parents)
        /// </summary>
        private void UpdateOwners() {
            foreach (var balance in this.DailyGoldBalances) {
                balance.UpdateOwners();
            }
            foreach (var stat in this.DailyTimeStatistics) {
                stat.UpdateOwners();
            }
        }

        /// <summary>
        /// loads all slayer tasks using threads
        /// </summary>
        private void LoadTasks() {
            string[] files = Directory.GetFiles(@"../../Tasks");
            Parallel.ForEach(files, path => LoadTaskFromJson(path));
            List<DailySlayerTaskList> tempList = new List<DailySlayerTaskList>(this.DailySlayerTasks);
            tempList = tempList.OrderByDescending(task => task.TaskDate).ToList();
            this.DailySlayerTasks = new ObservableCollection<DailySlayerTaskList>(tempList);
            this.Dispatcher.Invoke(() => slayerPage.SlayerTasksView.ItemsSource = this.DailySlayerTasks);
        }

        /// <summary>
        /// loads a single slayer task from a file
        /// </summary>
        /// <param name="path">
        /// path to a file containing the slayer task
        /// </param>
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

        /// <summary>
        /// load all daily gold balances using threads
        /// </summary>
        private void LoadBalances() {
            string[] files = Directory.GetFiles(@"../../MoneyBalances");
            Parallel.ForEach(files, path => LoadBalanceFromJson(path));
            List<DailyGoldBalance> tempList = new List<DailyGoldBalance>(this.DailyGoldBalances);
            tempList = tempList.OrderByDescending(balance => balance.Date).ToList();
            this.DailyGoldBalances = new ObservableCollection<DailyGoldBalance>(tempList);
            this.Dispatcher.Invoke(() => goldBalancePage.GoldBalanceView.ItemsSource = this.DailyGoldBalances);
        }

        /// <summary>
        /// Function to load a daily gold balance from json
        /// </summary>
        /// <param name="path">
        /// path to a file containing a daily gold balance
        /// </param>
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

        private void LoadTimeStatistics() {
            string[] files = Directory.GetFiles(@"../../TimeStatistics");
            Parallel.ForEach(files, path => this.LoadTimeStatisticFromJson(path));
            List<DailyTimeStatistic> tempList = new List<DailyTimeStatistic>(this.DailyTimeStatistics);
            tempList = tempList.OrderByDescending(statistic => statistic.Date).ToList();
            this.DailyTimeStatistics = new ObservableCollection<DailyTimeStatistic>(tempList);
            this.Dispatcher.Invoke(() => this.inGameTimePage.TimeStatisticsView.ItemsSource = this.DailyTimeStatistics);
        }

        private void LoadTimeStatisticFromJson(string path) {
            if (String.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;

            DailyTimeStatistic ingametime = null;

            using (var reader = new StreamReader(path)) {
                ingametime = JsonConvert.DeserializeObject<DailyTimeStatistic>(reader.ReadToEnd());
            }

            lock (statisticThreadLock) {
                this.DailyTimeStatistics.Add(ingametime);
            }
        }

        /// <summary>
        /// initializes application view
        /// </summary>
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
            item3.Header = "In-game time";

            this.slayerPage = new SlayerPage();
            f1.Content = slayerPage;
            
            this.goldBalancePage = new GoldBalancePage();
            f2.Content = goldBalancePage;

            this.inGameTimePage = new InGameTimePage();
            f3.Content = inGameTimePage;

            item1.Content = f1;
            item2.Content = f2;
            item3.Content = f3;

            this.Tabs.Items.Add(item1);
            this.Tabs.Items.Add(item2);
            this.Tabs.Items.Add(item3);
        }

        /// <summary>
        /// shows window to add a new Slayer task
        /// </summary>
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

        /// <summary>
        /// saves current progress
        /// </summary>
        private void SaveAllProgress() {
            (new Thread(() => Earnings.DumpToDisk())).Start();
            Parallel.ForEach(DailyGoldBalances, element => element.SaveToJson());
            Parallel.ForEach(DailySlayerTasks, element => element.SaveToJson());
            Parallel.ForEach(DailyTimeStatistics, element => element.SaveToJson());
            MessageBox.Show("Progress saved successfully!", "Saving", MessageBoxButton.OK);
        }

        /// <summary>
        /// draws a chart depending on which page is active (gold balance/slayer xp)
        /// </summary>
        public void DrawChart() {
            this.Dispatcher.Invoke(() => {
                if (this.Tabs.SelectedItem == null) return;
                if (this.Tabs.SelectedItem is TabItem ti) {
                    if (ti.Content is Frame f) {
                        if (f.Content is SlayerPage p1) {
                            p1.DrawChart();
                        } else if (f.Content is GoldBalancePage p2) {
                            p2.DrawChart();
                        } else if (f.Content is InGameTimePage p3) {
                            p3.DrawChart();
                        }
                    }
                }
            });
        }

        //event handlers
        private void OnWindowClose(object sender, System.ComponentModel.CancelEventArgs e) {
            MessageBoxResult choice = MessageBox.Show("Do you want to save unsaved changes?", "Save changes", MessageBoxButton.YesNoCancel);

            if (choice == MessageBoxResult.Yes) {
                this.SaveAllProgress();
            }

            if (choice == MessageBoxResult.Cancel) {
                e.Cancel = true;
                return;
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
