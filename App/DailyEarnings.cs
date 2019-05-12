using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunescapeOrganiser {
    public class DailyEarnings {
        public string Date {
            get;set;
        }

        public ObservableCollection<SoldItem> SoldItems {
            get;set;
        }

        public decimal TotalMoneyEarned {
            get;set;
        }

        public DailyEarnings() {
            this.SoldItems = new ObservableCollection<SoldItem>();
            DateTime dt = DateTime.Now;
            this.Date = String.Format("{0}/{1}/{2}", dt.Day, dt.Month, dt.Year);
        }

        public void Add(SoldItem item) {
            if (item == null) return;
            this.SoldItems.Add(item);
        }

        public bool Contains(SoldItem item) {
            return this.SoldItems.Contains(item);
        }

        // overrides and operators
        public override string ToString() {
            return base.ToString();
        }

        public override bool Equals(object obj) {
            if (obj is DailyEarnings earnings) {
                return earnings.Date == this.Date;
            }
            throw new InvalidOperationException();
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public static bool operator==(DailyEarnings d1, DailyEarnings d2) {
            return d1.Equals(d2);
        }

        public static bool operator!=(DailyEarnings d1, DailyEarnings d2) {
            return !d1.Equals(d2);
        }
    }
}
