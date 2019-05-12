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
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace RunescapeOrganiser {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        //TODO: add a chart drawing for earnings and slayer xp gained
        public ObservableCollection<DailySlayerTaskList> DailySlayerTasks {
            get;set;
        }

        public ObservableCollection<DailyEarnings> @DailyEarnings {
            get;set;
        }

        private static object threadLock = new object();
        private static object threadLock2 = new object();

        private bool FinishedLoading {
            get; set;
        }

        private bool FinishedLoading2 {
            get;set;
        }

        private SlayerPage slayerPage;
        private EarningsPage earningsPage;

        public MainWindow() {
            this.DailySlayerTasks = new ObservableCollection<DailySlayerTaskList>();
            this.@DailyEarnings = new ObservableCollection<DailyEarnings>();
            this.InitializeComponent();
            this.ApplicationInit();
            Slayer.InitSlayerTables();
            Earnings.InitItemNames();
            this.LoadTasks();
            GC.Collect();
        }

        private void LoadTasks() {
            string[] files = Directory.GetFiles(@"../../Tasks");
            List<Thread> threadList = new List<Thread>();
            foreach (var file in files) {
                Thread t = new Thread(() => LoadTaskFromJson(file));
                threadList.Add(t);
                t.Start();

            }
            while (!this.FinishedLoading) {
                this.FinishedLoading = threadList.All(t => !t.IsAlive);
            }
            List<DailySlayerTaskList> tempList = new List<DailySlayerTaskList>(this.DailySlayerTasks);
            tempList = tempList.OrderByDescending(task => task.TaskDate).ToList();
            this.DailySlayerTasks = new ObservableCollection<DailySlayerTaskList>(tempList);
            this.Dispatcher.Invoke(() => slayerPage.SlayerTasksView.ItemsSource = this.DailySlayerTasks);
        }

        private void LoadEarnings() {
            string[] files = Directory.GetFiles(@"../../Earnings");
            List<Thread> threads = new List<Thread>();
            foreach (var file in files) {
                Thread t = new Thread(() => LoadDailyEarningsFromJson(file));
                threads.Add(t);
                t.Start();
            }
            while (!this.FinishedLoading2) {
                this.FinishedLoading2 = threads.All(t => !t.IsAlive);
            }
            List<DailyEarnings> tempList = new List<DailyEarnings>(this.@DailyEarnings);
            tempList = tempList.OrderByDescending(earning => earning.Date).ToList();
            this.@DailyEarnings = new ObservableCollection<DailyEarnings>(tempList);
            this.Dispatcher.Invoke(() => earningsPage.EarningsView.ItemsSource = this.@DailyEarnings);
        }

        private void LoadDailyEarningsFromJson(string path) {
            if (String.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;
            DailyEarnings er = null;

            using (var reader = new StreamReader(path)) {
                er = JsonConvert.DeserializeObject<DailyEarnings>(reader.ReadToEnd());
            }

            lock (threadLock2) {
                @DailyEarnings.Add(er);
            }
        }

        private void LoadTaskFromJson(string path) {
            if (String.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;

            DailySlayerTaskList l = null;

            using (var reader = new StreamReader(path)) {
                l = JsonConvert.DeserializeObject<DailySlayerTaskList>(reader.ReadToEnd());
            }

            lock (threadLock) {
                DailySlayerTasks.Add(l);
            }
        }

        private void ApplicationInit() {
            TabItem item1, item2;
            item1 = new TabItem();
            item2 = new TabItem();
            Frame f1, f2;
            f1 = new Frame();
            f2 = new Frame();
            f1.Background = Brushes.LightGray;
            f2.Background = Brushes.LightGray;
            item1.Header = "Slayer tasks";
            item2.Header = "Earnings";
            slayerPage = new SlayerPage();
            f1.Content = slayerPage;
            earningsPage = new EarningsPage();
            f2.Content = earningsPage;
            item1.Content = f1;
            item2.Content = f2;
            Tabs.Items.Add(item1);
            Tabs.Items.Add(item2);
        }

        public void ShowAddTaskWindow() {
            try {
                var t = Application.Current.Windows.OfType<TaskAddWindow>().ToArray()[0];
            } catch (IndexOutOfRangeException) {
                var taskadd = new TaskAddWindow();
                taskadd.Show();
                taskadd.GetHideDelegate()();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            slayerPage.KillAndClearChartProcess();
            Earnings.DumpToDisk();
        }
    }
}
