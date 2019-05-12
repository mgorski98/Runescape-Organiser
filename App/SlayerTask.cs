using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunescapeOrganiser {
    public class SlayerTask {

        public SlayerMonsters MonsterType {
            get;set;
        }

        public string MonsterName {
            get;set;
        }

        public BossSlayerMonsters? BossMonsterType {
            get;set;
        }

        public string BossMonsterName {
            get;set;
        }

        public bool IsExtended {
            get;set;
        }

        public bool IsCancelled {
            get;set;
        }

        public uint Amount {
            get;set;
        }

        public decimal ExperienceGained {
            get;set;
        }

        public bool HasBossCounterpart {
            get;set;
        }

        public uint? BossKills {
            get;set;
        }

        public List<string> AdditionalNotes {
            get;set;
        }
        
        public uint TotalKills {
            get;set;
        }

        private DailySlayerTaskList owner;

        public SlayerTask() {}
        public SlayerTask(SlayerMonsters mType, uint amount, bool hasBoss = false, uint? bosskills = null, BossSlayerMonsters? bType = null, bool iscancelled = false, bool isextended = false, params string[] notes) {
            this.AdditionalNotes = new List<string>();
            this.MonsterType = mType;
            this.Amount = amount;
            this.HasBossCounterpart = hasBoss;
            this.BossKills = bosskills;
            this.BossMonsterType = bType;
            this.IsCancelled = iscancelled;
            this.IsExtended = isextended;
            if (this.MonsterType != SlayerMonsters.None) {
                this.MonsterName = Slayer.SlayerLookUpTable[this.MonsterType].Key;
            } else {
                throw new InvalidOperationException("No monster type chosen");
            }
            if (this.HasBossCounterpart && this.BossMonsterType != null && this.BossMonsterType != BossSlayerMonsters.None) {
                this.BossMonsterName = Slayer.BossSlayerLookUpTable[(BossSlayerMonsters)this.BossMonsterType].Key;
            }
            if (notes != null && notes.Length > 0) {
                this.AdditionalNotes.AddRange(notes);
            }
            this.TotalKills = this.Amount + (this.BossKills ?? 0);
        }

        public void InitExp(params SlayerBonuses[] bonuses) {
            this.ExperienceGained = this.CalculateExperience(bonuses);
        }

        private decimal CalculateExperience(params SlayerBonuses[] bonuses) {
            decimal result = 0m;
            decimal tempResult = 0m;
            result = this.Amount * Slayer.SlayerLookUpTable[this.MonsterType].Value;
            tempResult = result;

            if (this.HasBossCounterpart && this.BossKills != null && this.BossKills > 0 && this.BossMonsterType != null && this.BossMonsterType != BossSlayerMonsters.None) {
                result += ((uint)this.BossKills * Slayer.BossSlayerLookUpTable[(BossSlayerMonsters)this.BossMonsterType].Value);
            }

            if (bonuses != null && bonuses.Length > 0) {
                foreach (var bonus in bonuses) {
                    try {
                        result += (tempResult * Slayer.SlayerBonusesLookUpTable[bonus]);
                    } catch (KeyNotFoundException) { continue; }
                }
            }

            return result;
        }
        public void SetOwner(DailySlayerTaskList list) => this.owner = list;
        public DailySlayerTaskList GetOwner() => this.owner;
        

        //overrides
        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            sb.Append("Experience gained: ");
            sb.Append(this.ExperienceGained.ToString());
            sb.Append("xp\n");

            if (this.Amount > 0) {
                sb.Append("Monster name: ");
                sb.Append(this.MonsterName);
                sb.Append("\n");
                sb.Append("Kills: ");
                sb.Append(this.Amount.ToString());
                sb.Append("\n");
            }
            

            if (this.HasBossCounterpart && this.BossKills != null && this.BossKills > 0) {
                sb.Append("Boss name: ");
                sb.Append(this.BossMonsterName);
                sb.Append("\n");
                sb.Append("Kills: ");
                sb.Append(this.BossKills.ToString());
                sb.Append("\n");
            }

            sb.Append("Extended: ");
            sb.Append(this.IsExtended ? "Yes" : "No");
            sb.Append("\n");
            sb.Append("Cancelled: ");
            sb.Append(this.IsCancelled ? "Yes" : "No");
            sb.Append("\n");
            sb.Append("Additional notes: \n");
            if (this.AdditionalNotes.Count > 0) {
                foreach (var entry in this.AdditionalNotes) {
                    sb.Append("- ");
                    sb.Append(entry);
                    sb.Append("\n");
                }
            } else {
                sb.Append("None");
            }

            return sb.ToString();
        }
    }
}
