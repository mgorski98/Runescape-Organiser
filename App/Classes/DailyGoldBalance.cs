using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Utils;
using Newtonsoft.Json;

namespace RunescapeOrganiser {
    public class DailyGoldBalance : IJsonSerializable {

        public string Date {
            get;set;
        }

        [JsonIgnore]
        public ObservableCollection<GoldBalance> EarningsAndExpenses {
            get;set;
        }

        private DailyEarnings earnings;
        private DailyExpenses expenses;

        public DailyGoldBalance() {
            earnings = new DailyEarnings();
            expenses = new DailyExpenses();
            this.EarningsAndExpenses = new ObservableCollection<GoldBalance>() { earnings, expenses };
            foreach (var item in this.EarningsAndExpenses) {
                item.SetOwner(this);
            }
            this.Date = DateUtils.GetTodaysDate();
        }

        public void SaveToJson() {
            string path = @"../../MoneyBalances/" + "Gold balance from " + this.Date.Replace('/', '.') + @".bnc";
            using (var fs = new FileStream(path, FileMode.Create)) {
                using (var writer = new StreamWriter(fs)) {
                    writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
                }
            }
        }

        public decimal GetEarnings() {
            return (this.EarningsAndExpenses[0] as DailyEarnings)?.TotalMoneyEarned() ?? 0;
        }

        public decimal GetExpenses() {
            return (this.EarningsAndExpenses[1] as DailyExpenses)?.TotalMoneySpent() ?? 0;
        }

        public decimal TotalGoldBalance() {
            DailyExpenses expenses = this.EarningsAndExpenses[1] as DailyExpenses;
            DailyEarnings earnings = this.EarningsAndExpenses[0] as DailyEarnings;
            if (expenses == null || earnings == null) return 0m;
            return earnings.TotalMoneyEarned() - expenses.TotalMoneySpent();
        }

        //overrides and operators
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            DailyExpenses expenses = this.EarningsAndExpenses[1] as DailyExpenses;
            DailyEarnings earnings = this.EarningsAndExpenses[0] as DailyEarnings;
            if (expenses == null || earnings == null) return "";
            sb.Append("Total gold balance for ");
            sb.Append(this.Date);
            sb.Append(":\n");
            if (earnings.SoldItems.Count <= 0 && expenses.BoughtItems.Count <= 0) {
                sb.Append("No data");
            } else {
                if (earnings.SoldItems.Count > 0) {
                    sb.Append("- ");
                    sb.Append("Earnings: ");
                    sb.Append(earnings.TotalMoneyEarned().ToString("0.##"));
                    sb.Append("gp\n");
                }
                if (expenses.BoughtItems.Count > 0) {
                    sb.Append("- ");
                    sb.Append("Expenses: ");
                    sb.Append(expenses.TotalMoneySpent().ToString("0.##"));
                    sb.Append("gp\n");
                }
                sb.Append("Total balance: ");
                sb.Append(this.TotalGoldBalance().ToString("0.##"));
                sb.Append("gp\n");
            }
            return sb.ToString();
        }

    }
}
