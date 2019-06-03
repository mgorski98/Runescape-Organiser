using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunescapeOrganiser {
    public class TotalTimeStatistics {
        public Skill SkillType {
            get; set;
        }
        public string SkillName {
            get;set;
        }
        public decimal yearsSpent;
        public decimal monthsSpent;
        public decimal daysSpent;
        public decimal hoursSpent;
        public decimal minutesSpent;
        public decimal secondsSpent;
    }
}
