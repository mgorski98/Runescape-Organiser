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
    public class DailyEarnings : GoldBalance {
        public string Date {
            get;set;
        }

        public ObservableCollection<Item> SoldItems {
            get;set;
        }

        private DailyGoldBalance owner;

        public DailyEarnings() {
            this.SoldItems = new ObservableCollection<Item>();
            this.Date = DateUtils.GetTodaysDate();
        }

        public override void SetOwner(DailyGoldBalance gb) => this.owner = gb;
        public override DailyGoldBalance GetOwner() => this.owner;

        public void Add(Item item) {
            if (item == null) return;
            Item found = Find(item);
            if (found != null) {
                found.Add(item);
                return;
            }
            this.SoldItems.Add(item);
            item.SetOwner(this);
            this.SortDesc();
        }

        public void SortDesc() {
            List<Item> items = new List<Item>(this.SoldItems);
            items.Sort((i1, i2) => i1.ItemName.CompareTo(i2.ItemName));
            this.SoldItems = new ObservableCollection<Item>(items);
        }

        public void UpdateOwners() {
            foreach (var item in this.SoldItems) item.SetOwner(this);
        }

        public void Remove(Item item) {
            if (item == null) return;
            this.SoldItems.Remove(item);
        }

        public bool Contains(Item item) {
            return this.SoldItems.Contains(item);
        }

        public Item Find(Item item) {
            foreach (var _item in this.SoldItems) {
                if (_item.Equals(item)) {
                    return _item;
                }
            }
            return null;
        }

        public decimal TotalMoneyEarned() {
            return SoldItems?.Sum(si => si?.Price) ?? 0;
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
                sb.Append(this.TotalMoneyEarned().ToString("#,##0"));
                sb.Append("gp\n");
                sb.Append("Income sources: \n");
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
            int result = this.Date.Length.GetHashCode();
            unchecked {
                result *= 366 * (366 ^ this.SoldItems.GetHashCode());
            }
            return result;
        }

        public static bool operator==(DailyEarnings d1, DailyEarnings d2) {
            return d1.Equals(d2);
        }

        public static bool operator!=(DailyEarnings d1, DailyEarnings d2) {
            return !d1.Equals(d2);
        }
    }
}
