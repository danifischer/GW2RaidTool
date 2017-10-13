using System.Collections.Generic;

namespace RaidTool.Enums
{
	public static class LogFilter
	{
		public static Dictionary<LogFilterEnum, string> LogFilterDictionary = new Dictionary<LogFilterEnum, string>
		{
			{LogFilterEnum.All, "all"},
			{LogFilterEnum.Latest, "latest"},
			{LogFilterEnum.Succeeded, "succeeded"}
		};
	}

	public enum LogFilterEnum
	{
		All = 0,
		Latest = 1,
		Succeeded = 2
	}
}