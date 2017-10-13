namespace RaidTool.Logic.Interfaces
{
	public interface ILogfileParser
	{
		void ParseLogfile(string name, string path, string raidHerosPath);
	}
}