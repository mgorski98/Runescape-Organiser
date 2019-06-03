using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunescapeOrganiser {
    public class DailySkill {

        public Skill SkillType {
            get;set;
        }

        public string SkillName {
            get;set;
        }

        private DailyTimeStatistic owner;

        public DailySkill() {}

        public DailySkill(Skill s) {
            this.SkillType = s;
            this.SkillName = Enum.GetName(typeof(Skill), s);
        }

        public override bool Equals(object obj) => obj is DailySkill skill ? this.SkillName == skill.SkillName;

        public override int GetHashCode() {
            int result = 0;
            unchecked {
                result = (125 * (byte)SkillType) ^ 20;
                result *= (230 * SkillName.GetHashCode()) ^ 12;
            }
            return result;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("Skill: ");
            sb.Append(this.SkillName);
            sb.Append("\n");
            sb.Append("Time spent: ");
            sb.Append(String.Format("{0:hh\\:mm\\:ss}", this.GetOwner()?.TimeSpentForSkills[this.SkillType]));
            return sb.ToString();
        }

        public void SetOwner(DailyTimeStatistic owner) => this.owner = owner;
        public DailyTimeStatistic GetOwner() => this.owner;
    }
}
