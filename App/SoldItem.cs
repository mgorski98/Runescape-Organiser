using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunescapeOrganiser {
    public class SoldItem {
        public ulong Amount {
            get;set;
        }
        public string ItemName {
            get;set;
        }
        public decimal Price {
            get;set;
        }

        private DailyEarnings owner;

        public SoldItem(string name, ulong amount, decimal Price) {
            this.ItemName = name;
            this.Amount = amount;
            this.Price = Price;
        }

        public void Add(SoldItem si) {
            if (si == null || si.ItemName != this.ItemName) return;
            this.Price += si.Price;
            this.Amount += si.Amount;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("Item sold: ");
            sb.Append(this.ItemName);
            sb.Append("\n");
            sb.Append("Quantity: ");
            sb.Append(this.Amount.ToString());
            sb.Append("\n");
            sb.Append("Total price: ");
            sb.Append(this.Price.ToString());
            sb.Append("gp");
            return sb.ToString();
        }

        public void SetOwner(DailyEarnings newOwner) => this.owner = newOwner;
        public DailyEarnings GetOwner() => this.owner;

        public override bool Equals(object obj) {
            return ItemName.Equals(obj);
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
        public static bool operator==(SoldItem si1, SoldItem si2) {
            return si1?.Equals(si2) ?? false;
        }
        public static bool operator !=(SoldItem si1, SoldItem si2) {
            return !si1?.Equals(si2) ?? false;
        }
       
    }
}
