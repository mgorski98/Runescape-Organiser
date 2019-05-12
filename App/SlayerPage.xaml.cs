using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for SlayerPage.xaml
    /// </summary>
    public partial class SlayerPage : Page {

        private MainWindow mainWindow = null;
        private Process chartProcess = null;
        private Thread chartThread = null;

        public SlayerPage() {
            InitializeComponent();
            this.mainWindow = Application.Current.Windows.OfType<MainWindow>().ElementAt(0);
        }

        public DailySlayerTaskList AddDaily() {
            string dateString = DateUtils.GetTodaysDate();// String.Format("{0}/{1}/{2}", dt.Day < 10 ? "0" + dt.Day.ToString() : dt.Day.ToString(), dt.Month < 10 ? "0" + dt.Month.ToString() : dt.Month.ToString(), dt.Year);
            foreach (var entry in SlayerTasksView.Items) {
                if (entry is DailySlayerTaskList list) {
                    if (list.TaskDate == dateString) {
                        return list;
                    }
                }
            }
            var newDaily = new DailySlayerTaskList();
            var tempList = new List<DailySlayerTaskList>(mainWindow.DailySlayerTasks);
            tempList.Add(newDaily);
            tempList = tempList.OrderByDescending(s => s.TaskDate).ToList();
            mainWindow.DailySlayerTasks = new System.Collections.ObjectModel.ObservableCollection<DailySlayerTaskList>(tempList);
            SlayerTasksView.ItemsSource = mainWindow.DailySlayerTasks;
            SlayerTasksView.UpdateLayout();
            return newDaily;
        }

        public void DeleteTask() {
            object o = SlayerTasksView.SelectedItem;
            if (o == null) return;
            if (o is DailySlayerTaskList l) {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the daily?", "Delete Daily", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes) {
                    mainWindow.DailySlayerTasks.Remove(l);
                    SlayerTasksView.UpdateLayout();
                    TaskInfo.Text = "";
                }
            }
            if (o is SlayerTask task) {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Delete", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes) {
                    task.GetOwner().Remove(task);
                    SlayerTasksView.UpdateLayout();
                    TaskInfo.Text = "";
                }
            }
        }

        private void AddTaskWindowShowEvent(object sender, RoutedEventArgs e) {//add task//actually, should only show the window
            mainWindow.ShowAddTaskWindow();
        }

        private void AddDailyEvent(object sender, RoutedEventArgs e) {//add daily
            AddDaily();
        }

        private void DeleteEvent(object sender, RoutedEventArgs e) {//delete item
            DeleteTask();
        }

        private void SlayerTasksView_Selected(object sender, RoutedEventArgs e) {
            UpdateTaskInfoContents();
        }

        public void UpdateTaskInfoContents() {
            TaskInfo.Text = SlayerTasksView.SelectedItem?.ToString();
        }

        public void DrawChart() {
            var chartData = SlayerTasksView.Items.Cast<DailySlayerTaskList>();
            if (chartData.Count() <= 1) {
                MessageBox.Show("Not enough data to draw a chart", "ChartError", MessageBoxButton.OK);
                return;
            }
            chartProcess = new Process();
            StringBuilder args = new StringBuilder();
            args.Append(chartData.Count().ToString() + ' ');
            foreach (var element in chartData) {
                args.Append(element.TaskDate + ' ');
            }
            args.Append(chartData.Count().ToString() + ' ');
            foreach (var element in chartData) {
                args.Append(element.TotalExperience().ToString() + ' ');
            }
            chartProcess.StartInfo.FileName = @"..\..\PythonScripts\SlayerXPPlot.py";
            chartProcess.StartInfo.Arguments = args.ToString();
            chartProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            try {
                chartProcess.Start();
            } catch (Exception) {
                MessageBox.Show("Error: Cannot find a file SlayerXPPlot.py");
                return;
            }
            chartProcess.WaitForExit();
        }

        private void DrawChartEvent(object sender, RoutedEventArgs e) {
            if (chartThread != null && chartThread.IsAlive) return;
            chartThread = new Thread(() => DrawChart());
            chartThread.Start();
        }

        public void KillAndClearChartProcess() {
            try {
                mainWindow = null;
                chartProcess?.Kill();
                chartProcess?.Dispose();
                chartThread = null;
            } catch (Exception) { }
        }
    }
}
