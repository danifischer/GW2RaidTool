using RaidTool.Models;

namespace RaidTool.Logic.Interfaces
{
	public interface IHtmlFileWatcher
	{
		void CreateRaidHerosFile(IEncounterLog encounterLog);
	}
}