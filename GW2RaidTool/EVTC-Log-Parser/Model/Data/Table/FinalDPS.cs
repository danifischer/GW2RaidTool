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
		public int PowerAll { get; set; }
		public int PowerBoss { get; set; }
		public int CondiAll { get; set; }
		public int CondiBoss { get; set; }

		public int TotalAll => PowerAll + CondiAll;
		public int TotalBoss => PowerBoss + CondiBoss;

		public int Down { get; set; }
		public bool Dead { get; set; }
		public int? DeadTime { get; set; }
		public double FightDurationPlayer { get; set; }
	}
}