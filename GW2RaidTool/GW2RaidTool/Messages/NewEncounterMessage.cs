using RaidTool.Models;

namespace RaidTool.Messages
{
	public class NewEncounterMessage
	{
		public IEncounterLog EncounterLog { get; set; }

		public NewEncounterMessage(IEncounterLog encounterLog)
		{
			EncounterLog = encounterLog;
		}
	}
}