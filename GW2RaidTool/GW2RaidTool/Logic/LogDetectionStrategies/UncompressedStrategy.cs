using RaidTool.Helper;
using RaidTool.Logic.Interfaces;

namespace RaidTool.Logic.LogDetectionStrategies
{
	public class UncompressedStrategy : ILogDetectionStrategy
	{
		public string Name => "uncompressed";
		public string Filter => "*.evtc";
		public int WaitTime => 500;
		public bool CheckFile(string path)
		{
			return FileInUseChecker.CheckFile(path, WaitTime);
		}
	}
}