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

        public DailyEarnings earnings;
        public DailyExpenses expenses;

        public DailyGoldBalance() {
            this.earnings = new DailyEarnings();
            this.expenses = new DailyExpenses();
            this.EarningsAndExpenses = new ObservableCollection<GoldBalance>() { this.earnings, this.expenses };
            foreach (var item in this.EarningsAndExpenses) {
                item.SetOwner(this);
            }
            this.Date = DateUtils.GetTodaysDate();
        }

        public void UpdateOwners() {
            this.earnings.SetOwner(this);
            this.expenses.SetOwner(this);
            foreach (var item in this.earnings.SoldItems) item.SetOwner(this.earnings);
            foreach (var item in this.expenses.BoughtItems) item.SetOwner(this.expenses);
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
                    sb.Append(earnings.TotalMoneyEarned().ToString("#,##0"));
                    sb.Append("gp\n");
                }
                if (expenses.BoughtItems.Count > 0) {
                    sb.Append("- ");
                    sb.Append("Expenses: ");
                    sb.Append(expenses.TotalMoneySpent().ToString("#,##0"));
                    sb.Append("gp\n");
                }
                sb.Append("Total balance: ");
                sb.Append(this.TotalGoldBalance().ToString("#,##0"));
                sb.Append("gp\n");
            }
            return sb.ToString();
        }

    }
}
