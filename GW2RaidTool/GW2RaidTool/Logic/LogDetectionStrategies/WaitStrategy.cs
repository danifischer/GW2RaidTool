using RaidTool.Logic.Interfaces;
using RaidTool.Properties;

namespace RaidTool.Logic.LogDetectionStrategies
{
	public class WaitStrategy : ILogDetectionStrategy
	{
		public string Filter => "*.evtc*";
		public int WaitTime => int.Parse(Settings.Default.WaitTime);
	}
}