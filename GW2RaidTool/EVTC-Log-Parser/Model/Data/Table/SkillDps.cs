using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVTC_Log_Parser.Model.Data.Table
{
	public class SkillDps
	{
		public SkillDps(double fightDuration)
		{
			FightDuration = fightDuration;
		}

		public double FightDuration { get; }

		public string Name { get; set; }

		public int SkillId { get; set; }

		public double DPSAll => TotalAll / FightDuration;
		public double DPSBoss => TotalBoss / FightDuration;
		public long PowerAll { get; set; }
		public long PowerBoss { get; set; }
		public long CondiAll { get; set; }
		public long CondiBoss { get; set; }

		public long TotalAll => PowerAll + CondiAll;
		public long TotalBoss => PowerBoss + CondiBoss;
	}
}
