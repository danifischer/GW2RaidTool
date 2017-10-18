using System;
using System.Collections.Generic;
using EVTC_Log_Parser.Model.Data.Table;

namespace EVTC_Log_Parser.Model
{
	public class FinalDPS
	{
		public FinalDPS(double fightDuration)
		{
			FightDuration = fightDuration;
		}

		public double FightDuration { get; }

		public string Group { get; set; }
		public string Character { get; set; }
		public string Account { get; set; }
		public string Profession { get; set; }
		public double DPSAll => TotalAll / FightDuration;
		public double DPSBoss => TotalBoss / FightDuration;
		public long PowerAll { get; set; }
		public long PowerBoss { get; set; }
		public long CondiAll { get; set; }
		public long CondiBoss { get; set; }

		public long TotalAll => PowerAll + CondiAll;
		public long TotalBoss => PowerBoss + CondiBoss;

		public int Down { get; set; }
		public bool Dead { get; set; }
		public int? DeadTime { get; set; }
		public double FightDurationPlayer { get; set; }
		public IEnumerable<SkillDps> Skills { get; set; }
	}
}