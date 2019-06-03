using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace RunescapeOrganiser {
    public class DailyTimeStatistic {

        public string Date {
            get;set;
        }

        public Dictionary<Skill, TimeSpan> TimeSpentForSkills {
            get;set;
        }

        public ObservableCollection<DailySkill> DailySkills {
            get;set;
        }

        public DailyTimeStatistic() {
            this.TimeSpentForSkills = new Dictionary<Skill, TimeSpan>();
            this.DailySkills = new ObservableCollection<DailySkill>();
            this.Date = DateUtils.GetTodaysDate();
        }

        public void Init() {
            foreach (var entry in this.TimeSpentForSkills) {
                this.DailySkills.Add(new DailySkill(entry.Key));
            }
            this.DailySkills = new ObservableCollection<DailySkill>(new HashSet<DailySkill>(this.DailySkills));
        }

        public void AddSkillEntry(Skill skill) => this.TimeSpentForSkills.Add(skill, TimeSpan.Zero);

        public void AddSkillEntry(Skill s, TimeSpan span) {
            if (this.TimeSpentForSkills.ContainsKey(s)) {
                TimeSpan oldVal = this.TimeSpentForSkills[s];
                oldVal += span;
                this.TimeSpentForSkills[s] = oldVal;
                return;
            }
            this.TimeSpentForSkills.Add(s, span);
        }

        public void Add(DailySkill s) {
            DailySkill _s = Find(s);
            if (_s != null) return;
            this.DailySkills.Add(s);
            AddSkillEntry(s.SkillType);
            s.SetOwner(this);
        }

        public void Remove(DailySkill s) => this.DailySkills.Remove(s);


        public bool Contains(DailySkill s) => TimeSpentForSkills.Keys.Any(e => e == s.SkillType);

        public bool Contains(Skill s) => TimeSpentForSkills.Keys.Any(e => e == s);

        public void UpdateOwners() {
            foreach (var skill in this.DailySkills) {
                skill.SetOwner(this);
            }
        }

        public void SaveToJson() {
            string path = @"../../TimeStatistics/" + "Time Statistics From " + this.Date.Replace("/", ".") + @".tsm";
            using (var fs = new FileStream(path, FileMode.Create)) {
                using (var writer = new StreamWriter(fs)) {
                    writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
                }
            }
        }


        public DailySkill Find(DailySkill s) {
            if (s == null) return null;
            foreach (var skill in DailySkills) {
                if (skill.Equals(s)) return skill;
            }
            return null;
        }

        //overrides
        public override bool Equals(object obj) {
            if (obj is DailyTimeStatistic dts) {
                return this.Date == dts.Date;
            }
            return false;
        }

        public override int GetHashCode() {
            int result = 0;

            unchecked {
                result = (this.Date.GetHashCode() * 320 * 2) ^ 69;
                result *= this.TimeSpentForSkills.GetHashCode() ^ 30 * 23;
                result *= (this.DailySkills.GetHashCode() * 2) ^ 40;
            }

            return result;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("Overview of time spent on ingame activities from ").Append(this.Date).Append(":\n");
            foreach (var entry in this.TimeSpentForSkills) {
                sb.Append("- ");
                sb.Append(Enum.GetName(typeof(Skill), entry.Key)).Append(": ");
                sb.Append(String.Format("{0:hh\\:mm\\:ss}", entry.Value) + "\n");
            }
            return sb.ToString();
        }
    }
}
