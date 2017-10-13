using System.Collections.Generic;
using System.IO;

namespace RaidTool.Logic.Interfaces
{
	public interface IFileWatcher
	{
		FileSystemWatcher LogfileWatcher { get; }
		void Run();
		void ParseLogFile(FileInfo fileInfo);
		void ParseLogFiles(IEnumerable<FileInfo> fileInfos);
	}
}