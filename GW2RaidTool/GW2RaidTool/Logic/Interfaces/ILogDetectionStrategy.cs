using System;

namespace RaidTool.Logic.Interfaces
{
	public interface ILogDetectionStrategy
	{
		string Name { get; }
		string Filter { get; }
		int WaitTime { get; }
		bool CheckFile(string path);
	}
}