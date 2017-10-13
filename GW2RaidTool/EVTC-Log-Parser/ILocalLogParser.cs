using EVTC_Log_Parser.Model;

namespace EVTC_Log_Parser
{
	public interface ILocalLogParser
	{
		SharedValues ParseLog(string log);
	}
}