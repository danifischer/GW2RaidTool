using EVTC_Log_Parser.Model;
using RaidTool.Models;

namespace RaidTool.Logic.Interfaces
{
	public interface ILogFileConverter
	{
		void ConvertLog(IEncounterLog herosLog, SharedValues sharedValues);
	}
}