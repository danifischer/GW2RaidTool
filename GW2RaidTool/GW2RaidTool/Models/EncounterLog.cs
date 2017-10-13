using System;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace RaidTool.Models
{
	public class EncounterLog : ReactiveObject, IEncounterLog
	{
		private double _allDps;
		private double _bossDps;
		private DateTime _encounterDate;
		private string _encounterResult;
		private TimeSpan _encounterTime;
		private string _name;

		public EncounterLog(string name, string parsedLogPath, string evtcPath)
		{
			Name = name;
			ParsedLogPath = parsedLogPath;
			EvtcPath = evtcPath;
			ParseDate = DateTime.UtcNow;
			CharacterStatistics = new ObservableCollection<CharacterStatistics>();
		}

		public DateTime EncounterDate
		{
			get => _encounterDate;
			set => _encounterDate = this.RaiseAndSetIfChanged(ref _encounterDate, value);
		}

		public DateTime ParseDate { get; set; }

		public string Name
		{
			get => _name;
			set => _name = this.RaiseAndSetIfChanged(ref _name, value);
		}

		public string EncounterResult
		{
			get => _encounterResult;
			set => _encounterResult = this.RaiseAndSetIfChanged(ref _encounterResult, value);
		}

		public TimeSpan EncounterTime
		{
			get => _encounterTime;
			set => _encounterTime = this.RaiseAndSetIfChanged(ref _encounterTime, value);
		}

		public string ParsedLogPath { get; set; }
		public string EvtcPath { get; set; }

		public double BossDps
		{
			get => _bossDps;
			set => _bossDps = this.RaiseAndSetIfChanged(ref _bossDps, value);
		}

		public double AllDps
		{
			get => _allDps;
			set => _allDps = this.RaiseAndSetIfChanged(ref _allDps, value);
		}

		public ObservableCollection<CharacterStatistics> CharacterStatistics { get; }
	}
}