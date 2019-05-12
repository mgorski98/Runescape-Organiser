using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

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
            this.Date = String.Format("{0}/{1}/{2}", dt.Day < 10 ? "0" + dt.Day.ToString() : dt.Day.ToString(), dt.Month < 10 ? "0" + dt.Month.ToString() : dt.Month.ToString(), dt.Year);
        }

        public void Add(SoldItem item) {
            if (item == null) return;
            SoldItem found = Find(item);
            if (found != null) {
                found += item;
                return;
            }
            this.SoldItems.Add(item);
        }
        
        public void Remove(SoldItem item) {
            if (item == null) return;
            this.SoldItems.Remove(item);
        }

        public bool Contains(SoldItem item) {
            return this.SoldItems.Contains(item);
        }

        public SoldItem Find(SoldItem item) {
            foreach (var _item in this.SoldItems) {
                if (_item == item) {
                    return _item;
                }
            }
            return null;
        }

        private void SaveToJson() {
            string path = @"../../Earnings/" + "Earnings from " + this.Date + @".ern";
            using (var fs = new FileStream(path, FileMode.Create)) {
                using (var writer = new StreamWriter(fs)) {
                    writer.Write(JsonConvert.SerializeObject(this.SoldItems, Formatting.Indented));
                }
            }
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
