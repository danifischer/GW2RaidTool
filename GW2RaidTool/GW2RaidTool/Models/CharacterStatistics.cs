using System.Collections;
using System.Collections.Generic;

namespace RaidTool.Models
{
	public class CharacterStatistics
	{
		public string Name { get; set; }

		public string DisplayName { get; set; }

		public string Role { get; set; }

		public double BossDps { get; set; }

		public double BossDamage { get; set; }


		public double AllDps { get; set; }

		public double AllDamage { get; set; }

		public string Down { get; set; }

		public string Dead { get; set; }
		
		public IList<SkillDamage> Skills { get; set; } = new List<SkillDamage>();
	}
}
