using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace RunescapeOrganiser { 
    /// <summary>
    /// Interaction logic for InGameTimePage.xaml
    /// </summary>
    public partial class InGameTimePage : Page {

        private Stopwatch skillTimer;
        private DispatcherTimer dispatcherTimer;
        private MainWindow mainWindow;


        public InGameTimePage() {
            this.InitializeComponent();
            this.PopulateSkillBox();
            this.skillTimer = new Stopwatch();
            this.InitDispatcherTimer(new TimeSpan(0,0,0,0,10));
            this.dispatcherTimer.Start();
            this.mainWindow = Application.Current.Windows.OfType<MainWindow>().ElementAt(0);
        }

        private void PopulateSkillBox() {
            this.SkillTypesComboBox.ItemsSource = Enum.GetNames(typeof(Skill)).OrderBy(s => s);
            this.SkillTypesComboBox.SelectedItem = this.SkillTypesComboBox.Items.GetItemAt(0);
        }

        private void InitDispatcherTimer(TimeSpan interval) {
            this.dispatcherTimer = new DispatcherTimer();
            this.dispatcherTimer.Tick += TimerTick;
            this.dispatcherTimer.Interval = interval;
        }

        private void AddNewTimespan() {
            string skillName = SkillTypesComboBox.SelectedItem as string;
            Skill skillType = (Skill)Enum.Parse(typeof(Skill), skillName);
            DailySkill skill = new DailySkill(skillType);
            object o = this.TimeStatisticsView.SelectedItem;
            if (o == null) {
                var daily = this.AddDaily();
                daily.Add(skill);
                daily.AddSkillEntry(skillType, this.skillTimer.Elapsed);
            } else if (o is DailyTimeStatistic dts) {
                dts.Add(skill);
                dts.AddSkillEntry(skillType, this.skillTimer.Elapsed);
            } else if (o is DailySkill ds) {
                ds.GetOwner()?.Add(skill);
                ds.GetOwner()?.AddSkillEntry(skillType, this.skillTimer.Elapsed);
            }
            this.skillTimer.Reset();
        }

        private DailyTimeStatistic AddDaily() {
            string date = Utils.DateUtils.GetTodaysDate();
            foreach (var entry in this.mainWindow.DailyTimeStatistics) {
                if (date == entry.Date) return entry;
            }
            var newDaily = new DailyTimeStatistic();
            List<DailyTimeStatistic> tempList = new List<DailyTimeStatistic>(mainWindow.DailyTimeStatistics) { newDaily };
            tempList = tempList.OrderByDescending(s => s.Date).ToList();
            this.mainWindow.DailyTimeStatistics = new System.Collections.ObjectModel.ObservableCollection<DailyTimeStatistic>(tempList);
            this.TimeStatisticsView.ItemsSource = this.mainWindow.DailyTimeStatistics;
            this.TimeStatisticsView.Items.Refresh();
            this.TimeStatisticsView.UpdateLayout();
            return newDaily;
        }

        private void DeleteItem() {
            if (this.TimeStatisticsView.SelectedItem == null) return;
            object o = this.TimeStatisticsView.SelectedItem;
            if (o is DailyTimeStatistic dst) {
                MessageBoxResult deletdis = MessageBox.Show("Do you really want to delete this item?", "Delete item", MessageBoxButton.YesNo);
                if (deletdis == MessageBoxResult.Yes) {
                    this.mainWindow.DailyTimeStatistics.Remove(dst);
                }
            } else if (o is DailySkill ds) {
                MessageBoxResult deletdis = MessageBox.Show("Do you really want to delete this item?", "Delete item", MessageBoxButton.YesNo);
                if (deletdis == MessageBoxResult.Yes) {
                    ds.GetOwner()?.Remove(ds);
                }
            }
        }

        public void DrawChart() {
            try {
                var t = Application.Current.Windows.OfType<TimePlot>().ElementAt(0);
            } catch (ArgumentOutOfRangeException) {
                (new TimePlot()).Show();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (this.skillTimer.IsRunning) return;
            this.skillTimer.Start();
            this.SkillTypesComboBox.IsEnabled = false;
        }

        private void TimerTick(object sender, EventArgs e) {
            this.TimerBox.Text = String.Format("{0:hh\\:mm\\:ss}", this.skillTimer.Elapsed);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e) {
            if (!this.skillTimer.IsRunning) return;
            this.skillTimer.Stop();
            this.SkillTypesComboBox.IsEnabled = true;
        }

        private void SaveProgressButton_Click(object sender, RoutedEventArgs e) => this.AddNewTimespan();

        private void DeleteItemButton_Click(object sender, RoutedEventArgs e) => this.DeleteItem();

        private void AddDailyButton_Click(object sender, RoutedEventArgs e) => this.AddDaily();

        private void SkillTypesComboBox_DropDownClosed(object sender, EventArgs e) => this.skillTimer.Reset();

        private void TimeStatisticsView_Selected(object sender, RoutedEventArgs e) {
            this.InfoBox.Text = this.TimeStatisticsView.SelectedItem?.ToString();
        }
    }
}
