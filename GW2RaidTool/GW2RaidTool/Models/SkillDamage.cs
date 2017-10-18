namespace RaidTool.Models
{
	public class SkillDamage
	{
		public string Name { get; set; }

		public int SkillId { get; set; }

		public long DamageBoss { get; set; }
		public long DamageAll { get; set; }

		public double DpsBoss { get; set; }
		public double DpsAll { get; set; }
	}
}
