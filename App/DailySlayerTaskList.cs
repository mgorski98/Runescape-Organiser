using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Utils;

namespace RunescapeOrganiser {
    public class DailySlayerTaskList {

        public ObservableCollection<SlayerTask> SlayerTasks {
            get;set;
        }

        public string TaskDate {
            get;set;
        }

        public DailySlayerTaskList() {
            this.SlayerTasks = new ObservableCollection<SlayerTask>();
            DateTime dt = DateTime.Now;
            this.TaskDate = DateUtils.GetTodaysDate();//String.Format("{0}/{1}/{2}", dt.Day < 10 ? "0" + dt.Day.ToString() : dt.Day.ToString(), dt.Month < 10 ? "0" + dt.Month.ToString() : dt.Month.ToString(), dt.Year);
        }

        ~DailySlayerTaskList() {
            this.SaveToJson();
            this.TaskDate = null;
        }

        public void Add(SlayerTask task) {
            if (task == null) return;
            this.SlayerTasks.Add(task);
            task.SetOwner(this);
        }

        public void Remove(SlayerTask task) {
            if (task == null) return;
            this.SlayerTasks.Remove(task);
        }

        public void SaveToJson() {
            string path = @"../../Tasks/" + "Tasks From " + this.TaskDate.Replace('/', '.') + ".tsk";
            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate)) {
                using (var writer = new System.IO.StreamWriter(fs)) {
                    writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
                }
            }
        }

        public decimal TotalExperience() {
            return SlayerTasks.Sum(task => task.ExperienceGained);
        }

        public ulong TotalKills() {
            return (ulong)SlayerTasks.Sum(task => task.Amount + (task.BossKills ?? 0));
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("Overview of tasks from ");
            sb.Append(this.TaskDate);
            sb.Append(":");
            sb.Append("\n");
            sb.Append("Number of tasks done: ");
            sb.Append(this.SlayerTasks.Count.ToString());
            sb.Append("\n");
            sb.Append("Total experience gained: ");
            sb.Append(TotalExperience().ToString());
            sb.Append("xp\n");
            Dictionary<string, uint> kills = new Dictionary<string, uint>();
            Dictionary<string, uint> bossKills = new Dictionary<string, uint>();
            foreach (var task in SlayerTasks) {
                try {
                    kills[task.MonsterName] = 0;
                } catch (ArgumentException) { continue; }
                if (task.HasBossCounterpart && task.BossKills != null) {
                    try {
                        bossKills[task.BossMonsterName] = 0;
                    } catch (ArgumentException) { continue; }
                }
            }
            foreach (var task in SlayerTasks) {
                kills[task.MonsterName] += task.Amount;
                if (task.HasBossCounterpart && task.BossKills != null) {
                    bossKills[task.BossMonsterName] += task.BossKills.Value;
                }
            }
            if (kills.Count > 0 && kills.Any(entry => entry.Value != 0)) {
                sb.Append("Monsters killed: \n");
                foreach (var entry in kills) {
                    if (entry.Value == 0) continue;
                    sb.Append("- ");
                    sb.Append(entry.Key);
                    sb.Append(": ");
                    sb.Append(entry.Value.ToString());
                    sb.Append(" kills\n");
                }
            }
            
            if (bossKills.Count > 0 && bossKills.Any(entry => entry.Value != 0)) {
                sb.Append("Bosses killed: \n");
                foreach (var entry in bossKills) {
                    if (entry.Value == 0) continue;
                    sb.Append("- ");
                    sb.Append(entry.Key);
                    sb.Append(": ");
                    sb.Append(entry.Value.ToString());
                    sb.Append(" kills\n");
                }
            }

            sb.Append("Total monsters killed: ");
            sb.Append(this.TotalKills().ToString());

            return sb.ToString();
        }
    }
}
