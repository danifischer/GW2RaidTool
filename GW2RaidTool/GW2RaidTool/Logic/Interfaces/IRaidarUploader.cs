using RaidTool.Models;

namespace RaidTool.Logic.Interfaces
{
	public interface IRaidarUploader
	{
		void Upload(IEncounterLog encounterLog);
	}
}