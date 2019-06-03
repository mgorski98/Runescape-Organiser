using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utils;

namespace RunescapeOrganiser
{
    public class TimeViewModel
    {

        private MainWindow mainWindow;

        public PlotModel TimePlotModel {
            get; set;
        }

        public TimeViewModel() {
            this.mainWindow = Application.Current.Windows.OfType<MainWindow>().ElementAt(0);
            this.SetUpViewModel();
        }

        private Dictionary<Skill, TimeSpan> GetTotalSkillTime(ObservableCollection<DailyTimeStatistic> collection) {
            var result = new Dictionary<Skill, TimeSpan>();

            foreach (var value in Enum.GetValues(typeof(Skill)).Cast<Skill>()) {
                result.Add(value, TimeSpan.Zero);
            }

            foreach (var stat in collection) {
                foreach (var entry in stat.TimeSpentForSkills) {
                    TimeSpan oldValue = result[entry.Key];
                    oldValue += entry.Value;
                    result[entry.Key] = oldValue;
                }
            }

            return result;
        }

        private void SetUpViewModel() {
            TimePlotModel = new PlotModel() { Title = "Time spent on certain skills (in hours)" };
            var statistics = mainWindow.DailyTimeStatistics;
            var plotData = GetTotalSkillTime(statistics);

            var barSeries = new BarSeries() {
                ItemsSource = plotData.Select(entry => new BarItem() { Value = entry.Value.TotalHours })
            };

            TimePlotModel.Series.Add(barSeries);
            TimePlotModel.Axes.Add(new CategoryAxis() {
                Position = AxisPosition.Left,
                Key = "Skills",
                ItemsSource = Enum.GetNames(typeof(Skill))
            });
        }
    }
}
