using RaidTool.Logic.Interfaces;

namespace RaidTool.Logic.LogDetectionStrategies
{
	public class CompressedStrategy : ILogDetectionStrategy
	{
		public string Filter => "*.evtc.zip";
		public int WaitTime => 500;
	}
}