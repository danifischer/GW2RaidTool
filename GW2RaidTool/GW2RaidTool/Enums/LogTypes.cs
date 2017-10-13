using System.Collections.Generic;

namespace RaidTool.Enums
{
	public static class LogTypes
	{
		public static Dictionary<LogTypesEnum, string> LogTypesDictionary = new Dictionary<LogTypesEnum, string>
		{
			{LogTypesEnum.AutoDetect, "auto detect"},
			{LogTypesEnum.Normal, "normal"},
			{LogTypesEnum.Compressed, "compressed"}
		};
	}

	public enum LogTypesEnum
	{
		AutoDetect = 0,
		Normal = 1,
		Compressed = 2
	}
}