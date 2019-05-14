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
    public class DailyExpenses : IJsonSerializable {
        public string Date {
            get;set;
        }

        public ObservableCollection<Item> BoughtItems {
            get;set;
        }

        public DailyExpenses() {
            this.BoughtItems = new ObservableCollection<Item>();
            this.Date = DateUtils.GetTodaysDate();
        }

        public void SaveToJson() {
            string path = @"../../Expenses/" + "Expenses from " + this.Date.Replace('/', '.') + @".exp";
            using (var fs = new FileStream(path, FileMode.Create)) {
                using (var writer = new StreamWriter(fs)) {
                    writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
                }
            } 
        }
    }
}
