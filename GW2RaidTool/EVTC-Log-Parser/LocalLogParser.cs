using System.IO;
using EVTC_Log_Parser.Model;

namespace EVTC_Log_Parser
{
	public class LocalLogParser : ILocalLogParser
	{
		public SharedValues ParseLog(string log)
		{
			var parser = new Parser();

			if (parser.Parse(log))
			{
				var converter = new Converter(parser);
				return converter.GetFinalDPS();
			}
			else
			{
				throw new IOException("Could nor tread or parse log");
			}
		}
	}
}