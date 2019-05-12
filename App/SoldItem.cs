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
        public SoldItem(string name, ulong amount, decimal Price) {
            this.ItemName = name;
            this.Amount = amount;
            this.Price = Price;
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
            return sb.ToString();
        }
        public override bool Equals(object obj) {
            return ItemName.Equals(obj);
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
        public static bool operator==(SoldItem si1, SoldItem si2) {
            return si1.Equals(si2);
        }
        public static bool operator !=(SoldItem si1, SoldItem si2) {
            return !si1.Equals(si2);
        }
        public static SoldItem operator+(SoldItem si1, SoldItem si2) {
            return si1 == si2 ? new SoldItem(si1.ItemName, si1.Amount + si2.Amount, si1.Price + si2.Price) : null;
        }
    }
}
