using System;
using System.IO;
using System.Threading;
using IniParser;
using Microsoft.Win32;
using RaidTool.Helper;
using RaidTool.Logic.Interfaces;
using RaidTool.Messages;
using RaidTool.Properties;
using ReactiveUI;

namespace RaidTool.Logic.LogDetectionStrategies
{
	public class IniReaderStrategy : ILogDetectionStrategy
	{
		private readonly IMessageBus _messageBus;

		public IniReaderStrategy(IMessageBus messageBus)
		{
			_messageBus = messageBus;
			Filter = DetermineFilter();
			WaitTime = DetermineWaitTime();
		}

		public string Name => "auto detect (read ArcDps ini)";

		public string Filter { get; set; }

		public int WaitTime { get; set; }

		public bool CheckFile(string path)
		{
			return FileInUseChecker.CheckFile(path, WaitTime);
		}

		private string DetermineFilter()
		{
			try
			{
				var registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Gw2\shell\open\command", false);
				if (registryKey != null)
				{
					var value = registryKey.GetValue("").ToString();
					var replace = value.Replace("\\", "/").Replace(@"\", "").Replace("\"", "").Replace("/", @"\");
					var substring = replace.Substring(0, replace.IndexOf("%", StringComparison.Ordinal));

					if (File.Exists(substring))
					{
						var directoryName = Path.GetDirectoryName(substring);
						if (directoryName != null)
						{
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
								return "*.evtc*.zip";
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				_messageBus.SendMessage(new LogMessage($"Could not determine ArcDps setting (using wait strategy): {e.Message}"));
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