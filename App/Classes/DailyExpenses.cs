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
    public class DailyExpenses : GoldBalance {
        public string Date {
            get;set;
        }

        public ObservableCollection<Item> BoughtItems {
            get;set;
        }

        private DailyGoldBalance owner;

        public override void SetOwner(DailyGoldBalance owner) => this.owner = owner;
        public override DailyGoldBalance GetOwner() => this.owner;

        public DailyExpenses() {
            this.BoughtItems = new ObservableCollection<Item>();
            this.Date = DateUtils.GetTodaysDate();
        }

        public void Add(Item item) {
            if (item == null) return;
            Item i = Find(item);
            if (i != null) {
                i.Add(item);
                return;
            }
            this.BoughtItems.Add(item);
            item.SetOwner(this);
            this.SortDesc();
        }

        public void Remove(Item item) {
            if (item == null) return;
            this.BoughtItems.Remove(item);
        }

        public void SortDesc() {
            List<Item> items = new List<Item>(this.BoughtItems);
            items.Sort((i1, i2) => i1.ItemName.CompareTo(i2.ItemName));
            this.BoughtItems = new ObservableCollection<Item>(items);
        }

        public Item Find(Item item) {
            foreach (var _item in this.BoughtItems) {
                if (_item.ItemName == item.ItemName) {
                    return _item;
                }
            }
            return null;
        }

        public decimal TotalMoneySpent() {
            return this.BoughtItems.Sum(item => item.Price);
        }

        public void SaveToJson() {
            string path = @"../../Expenses/" + "Expenses from " + this.Date.Replace('/', '.') + @".exp";
            using (var fs = new FileStream(path, FileMode.Create)) {
                using (var writer = new StreamWriter(fs)) {
                    writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
                }
            } 
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("Overview of expenses from ");
            sb.Append(this.Date);
            sb.Append(":\n");
            if (this.BoughtItems.Count > 0) {
                sb.Append("Total money spent: ");
                sb.Append(this.TotalMoneySpent().ToString("#,##0"));
                sb.Append("gp\n");
                sb.Append("Items bought: \n");
                foreach (var item in this.BoughtItems) {
                    sb.Append(item.ToInfoString());
                }
            } else {
                sb.Append("None (yet)");
            }
            return sb.ToString();
        }


        public override bool Equals(object obj) {
            if (obj is DailyExpenses ex) {
                return ex?.Date == this.Date;
            }
            return false;
        }

        public override int GetHashCode() {
            int result = this.Date.Length.GetHashCode();

            unchecked {
                result *= 420 * (420 ^ (this.BoughtItems.GetHashCode()));
            }

            return result;
        }

        public static bool operator==(DailyExpenses ex1, DailyExpenses ex2) {
            return ex1?.Equals(ex2) ?? false;
        }

        public static bool operator!=(DailyExpenses ex1, DailyExpenses ex2) {
            return !ex1?.Equals(ex2) ?? false;
        }
    }
}
