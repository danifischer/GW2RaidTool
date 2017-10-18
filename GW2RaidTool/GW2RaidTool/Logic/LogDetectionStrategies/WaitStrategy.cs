using System.IO;
using System.Threading;
using RaidTool.Helper;
using RaidTool.Logic.Interfaces;
using RaidTool.Properties;

namespace RaidTool.Logic.LogDetectionStrategies
{
	public class WaitStrategy : ILogDetectionStrategy
	{
		public string Name => "auto detect (wait)";
		public string Filter => "*.evtc*";
		public int WaitTime => int.Parse(Settings.Default.WaitTime);
		public bool CheckFile(string path)
		{
			Thread.Sleep(WaitTime);

			if (File.Exists(path))
			{
				return FileInUseChecker.CheckFile(path, 500);
			}

			return File.Exists(string.Concat(path, ".evtc")) && FileInUseChecker.CheckFile(string.Concat(path, ".evtc"), 500);
		}
	}
}