using System;
using System.Collections.ObjectModel;

namespace RaidTool.Models
{
	public interface IEncounterLog
	{
		DateTime EncounterDate { get; set; }
		DateTime ParseDate { get; set; }
		string Name { get; set; }
		string EncounterResult { get; set; }
		TimeSpan EncounterTime { get; set; }
		string ParsedLogPath { get; set; }
		string EvtcPath { get; set; }
		double BossDps { get; set; }
		double AllDps { get; set; }
		ObservableCollection<CharacterStatistics> CharacterStatistics { get; }

		bool? UploadComplete { get; set; }
	}
}