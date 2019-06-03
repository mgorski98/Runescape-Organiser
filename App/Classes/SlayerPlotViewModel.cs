using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace RunescapeOrganiser {
    public class SlayerPlotViewModel {

        private SlayerPage slayerPage;

        public PlotModel SlayerViewModel {
            get;set;
        }

        public SlayerPlotViewModel() {
            this.slayerPage = Application.Current.Windows.OfType<MainWindow>().ElementAt(0).GetSlayerPage();
            this.SetUpViewModel();
        }

        private void SetUpViewModel() {
            this.SlayerViewModel = new PlotModel() { Title = "Slayer XP gained" };

            var dateFormat = @"yyyy/MM/dd";

            var points = this.slayerPage.SlayerTasksView.ItemsSource.
                Cast<DailySlayerTaskList>().
                Select(x => new DataPoint(DateTimeAxis.ToDouble(DateTime.ParseExact(x.TaskDate, dateFormat, CultureInfo.CurrentCulture)), (double)x.TotalExperience())).
                ToArray();

            var lineSeries = new LineSeries() { ItemsSource = points, Color = OxyColors.Black, Title = "Slayer XP" };

            var dateTimeAxis = new DateTimeAxis() { Position = AxisPosition.Bottom };

            var xpAxis = new LinearAxis() { Position = AxisPosition.Left };

            this.SlayerViewModel.Axes.Add(dateTimeAxis);
            this.SlayerViewModel.Axes.Add(xpAxis);
            this.SlayerViewModel.Series.Add(lineSeries);
            this.SlayerViewModel.LegendTitle = "Legend";
            this.SlayerViewModel.LegendPosition = LegendPosition.LeftTop;
            
        }
    }
}
