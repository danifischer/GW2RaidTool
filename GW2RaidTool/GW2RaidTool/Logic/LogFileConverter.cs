using System;
using System.Linq;
using EVTC_Log_Parser.Model;
using RaidTool.Logic.Interfaces;
using RaidTool.Models;

namespace RaidTool.Logic
{
	public class LogFileConverter : ILogFileConverter
	{
		public void ConvertLog(IEncounterLog herosLog, SharedValues sharedValues)
		{
			herosLog.Name = sharedValues.Target;
			herosLog.EncounterDate = sharedValues.LogStart.ToLocalTime();
			herosLog.EncounterResult = sharedValues.Success ? "Success" : "Fail";

			var time = TimeSpan.FromSeconds(sharedValues.FightDuration);
			herosLog.EncounterTime = new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);

			foreach (var sharedValuesPlayerValue in sharedValues.PlayerValues)
			{
				var characterStats = new CharacterStatistics()
				{
					Name = sharedValuesPlayerValue.Character,
					DisplayName = sharedValuesPlayerValue.Account.Substring(1),
					BossDps = Math.Round(sharedValuesPlayerValue.DPSBoss),
					BossDamage = sharedValuesPlayerValue.TotalBoss,
					AllDps = Math.Round(sharedValuesPlayerValue.DPSAll),
					AllDamage = sharedValuesPlayerValue.TotalAll,
					Down = sharedValuesPlayerValue.Down.ToString(),
					Role = sharedValuesPlayerValue.Profession
				};

				if (sharedValuesPlayerValue.Dead 
				    && sharedValuesPlayerValue.FightDurationPlayer + 1.0 < sharedValues.FightDuration)
				{
					var percent = (sharedValuesPlayerValue.FightDurationPlayer / sharedValues.FightDuration * 100).ToString("0");
					var timeOfDead = TimeSpan.FromSeconds(sharedValuesPlayerValue.FightDurationPlayer);
					characterStats.Dead = $"{timeOfDead.Minutes}m {timeOfDead.Seconds}s ({percent}% alive)";
				}
				
				herosLog.CharacterStatistics.Add(characterStats);
			}

			herosLog.BossDps =
				Math.Round(sharedValues.PlayerValues.Sum(i => i.TotalBoss) / sharedValues.FightDuration);

			herosLog.AllDps =
				Math.Round(sharedValues.PlayerValues.Sum(i => i.TotalAll) / sharedValues.FightDuration);
		}
	}
}