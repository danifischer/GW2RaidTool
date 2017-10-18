namespace RaidTool.Logic.Interfaces
{
	public interface ILogfileParser
	{
		bool ParseLogfile(string name, string evtcPath, string outputPath, string raidHerosPath);
	}
}