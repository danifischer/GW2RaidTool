using System.IO;
using IniParser;
using Microsoft.Win32;
using RaidTool.Logic.Interfaces;
using RaidTool.Properties;

namespace RaidTool.Logic.LogDetectionStrategies
{
	public class IniReaderStrategy : ILogDetectionStrategy
	{
		public IniReaderStrategy()
		{
			Filter = DetermineFilter();
			WaitTime = DetermineWaitTime();
		}

		public string Filter { get; set; }

		public int WaitTime { get; set; }

		private string DetermineFilter()
		{
			var OurKey = Registry.LocalMachine;
			OurKey = OurKey.OpenSubKey(@"SOFTWARE\Classes\Gw2\shell\open\command", false);

			var valueNames = OurKey.GetValueNames();
			var value = OurKey.GetValue("").ToString();

			var replace = value.Replace("\\", "/").Replace(@"\", "").Replace("\"", "").Replace("/", @"\");
			var substring = replace.Substring(0, replace.IndexOf("%"));

			if (File.Exists(substring))
			{
				var directoryName = Path.GetDirectoryName(substring);
				var combine = Path.Combine(directoryName, @"addons\arcdps");
				if (File.Exists(string.Concat(combine, @"\arcdps.ini")))
				{
					var fileIniDataParser = new FileIniDataParser();
					var readFile = fileIniDataParser.ReadFile(string.Concat(combine, @"\arcdps.ini"));

					var logCompressed = readFile["session"]["boss_encounter_compress"];

					if (logCompressed == "0")
					{
						return "*.evtc";
					}
					return "*.evtc.zip";
				}
			}

			return "*.evtc*";
		}

		private int DetermineWaitTime()
		{
			if (Filter == null || Filter == ".evtc*")
			{
				return int.Parse(Settings.Default.WaitTime);
			}
			return 500;
		}
	}
}