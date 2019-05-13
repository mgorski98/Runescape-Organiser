using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Utils;

namespace RunescapeOrganiser {
    public class DailyEarnings : IJsonSerializable {
        public string Date {
            get;set;
        }

        public ObservableCollection<SoldItem> SoldItems {
            get;set;
        }

        public DailyEarnings() {
            this.SoldItems = new ObservableCollection<SoldItem>();
            this.Date = DateUtils.GetTodaysDate();
        }

        ~DailyEarnings() {
            this.SaveToJson();
            this.Date = null;
            this.SoldItems = null;
        }

        public void Add(SoldItem item) {
            if (item == null) return;
            SoldItem found = Find(item);
            if (found != null) {
                found.Add(item);
                return;
            }
            this.SoldItems.Add(item);
            item.SetOwner(this);
            this.SortDesc();
        }

        public void SortDesc() {
            List<SoldItem> items = new List<SoldItem>(this.SoldItems);
            items.Sort((i1, i2) => i1.ItemName.CompareTo(i2.ItemName));
            this.SoldItems = new ObservableCollection<SoldItem>(items);
        }

        public void UpdateOwners() {
            foreach (var item in this.SoldItems) item.SetOwner(this);
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
                if (_item.ItemName == item.ItemName) {
                    return _item;
                }
            }
            return null;
        }

        public decimal TotalMoneyEarned() {
            return SoldItems.Sum(si => si.Price);
        }

        public void SaveToJson() {
            string path = @"../../Earnings/" + "Earnings from " + this.Date.Replace("/", ".") + ".ern";
            using (var fs = new FileStream(path, FileMode.Create)) {
                using (var writer = new StreamWriter(fs)) {
                    writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
                }
            }
        }

        // overrides and operators
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("Overview of earnings from ");
            sb.Append(this.Date);
            sb.Append(":\n");
            if (this.SoldItems.Count > 0) {
                sb.Append("Total money earned: ");
                sb.Append(this.TotalMoneyEarned().ToString("0.##"));
                sb.Append("gp\n");
                sb.Append("Items sold: \n");
                foreach (var item in SoldItems) {
                    sb.Append(item.ToInfoString());
                }
            } else {
                sb.Append("None (yet)");
            }
            return sb.ToString();
        }

        public override bool Equals(object obj) {
            if (obj is DailyEarnings earnings) {
                return earnings?.Date == this.Date;
            }
            return false;
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
