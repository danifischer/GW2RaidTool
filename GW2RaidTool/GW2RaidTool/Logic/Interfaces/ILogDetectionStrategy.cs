namespace RaidTool.Logic.Interfaces
{
	public interface ILogDetectionStrategy
	{
		string Filter { get; }
		int WaitTime { get; }
	}
}