using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RunescapeOrganiser {
    public class BalancePlotViewModel {

        private GoldBalancePage goldBalancePage;

        public PlotModel BalancePlotModel {
            get;set;
        }

        public BalancePlotViewModel() {
            this.goldBalancePage = Application.Current.Windows.OfType<MainWindow>().ElementAt(0).GetGoldBalancePage();
            this.SetUpViewModel();
        }

        private void SetUpViewModel() {
            this.BalancePlotModel = new PlotModel() { Title = "Daily gold balance" };

            var dateFormat = @"yyyy/MM/dd";

            var dataBase = this.goldBalancePage.GoldBalanceView.ItemsSource.Cast<DailyGoldBalance>();

            var expensesData = dataBase.
                Select(dgb => dgb.EarningsAndExpenses.OfType<DailyExpenses>().ElementAt(0)).
                Select(elem => new DataPoint(DateTimeAxis.ToDouble(DateTime.ParseExact(elem.Date,dateFormat, CultureInfo.CurrentCulture)), (double)elem.TotalMoneySpent())).
                ToArray();

            var earningsData = dataBase.
                Select(dgb => dgb.EarningsAndExpenses.OfType<DailyEarnings>().ElementAt(0)).
                Select(elem => new DataPoint(DateTimeAxis.ToDouble(DateTime.ParseExact(elem.Date, dateFormat, CultureInfo.CurrentCulture)), (double)elem.TotalMoneyEarned())).
                ToArray();

            var balanceData = dataBase.
                Select(x => new DataPoint(
                    DateTimeAxis.ToDouble(DateTime.ParseExact(x.Date, dateFormat, CultureInfo.CurrentCulture)), 
                    (double)(x.TotalGoldBalance())  
                )).
                ToArray();    

            var expensesSeries = new LineSeries() { Title = "Expenses", Color = OxyColors.Red, ItemsSource = expensesData };
            var earningsSeries = new LineSeries() { Title = "Earnings", Color = OxyColors.LimeGreen, ItemsSource = earningsData };
            var balanceSeries = new LineSeries() { Title = "Balance", Color = OxyColors.Gold, ItemsSource = balanceData };

            this.BalancePlotModel.Series.Add(expensesSeries);
            this.BalancePlotModel.Series.Add(earningsSeries);
            this.BalancePlotModel.Series.Add(balanceSeries);

            this.BalancePlotModel.Axes.Add(new DateTimeAxis() { Position = AxisPosition.Bottom });

            this.BalancePlotModel.Axes.Add(new LinearAxis() { Position = AxisPosition.Left });
        }
    }
}
