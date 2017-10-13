using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EVTC_Log_Parser;
using RaidTool.Logic.Interfaces;
using RaidTool.Messages;
using RaidTool.Models;
using RaidTool.Properties;
using ReactiveUI;

namespace RaidTool.Logic
{
	public class FileWatcher : IFileWatcher
	{
		private readonly Func<IHtmlFileWatcher> _htmlFileWatcherFactory;
		private readonly Func<ILocalLogParser> _localLogParserFactory;
		private readonly ILogFileConverter _logFileConverter;
		private readonly IMessageBus _messageBus;

		public FileWatcher(ILogFileConverter logFileConverter,
			Func<ILocalLogParser> localLogParserFactory,
			Func<IHtmlFileWatcher> htmlFileWatcherFactoryFactory,
			IMessageBus messageBus)
		{
			_logFileConverter = logFileConverter;
			_localLogParserFactory = localLogParserFactory;
			_htmlFileWatcherFactory = htmlFileWatcherFactoryFactory;
			_messageBus = messageBus;
		}

		public FileSystemWatcher LogfileWatcher { get; private set; }

		public void Run()
		{
			var logDict = Path.Combine(Environment.GetFolderPath(
				Environment.SpecialFolder.MyDocuments), @"Guild Wars 2\addons\arcdps\arcdps.cbtlogs\");
			if (!Directory.Exists(logDict))
				Directory.CreateDirectory(logDict);

			LogfileWatcher = new FileSystemWatcher(logDict)
			{
				IncludeSubdirectories = true,
				Filter = "*.evtc*"
			};
			LogfileWatcher.Created += LogFileOnCreated;
			LogfileWatcher.EnableRaisingEvents = true;
		}

		public void ParseLogFile(FileInfo fileInfo)
		{
			try
			{
				_messageBus.SendMessage(new LogMessage($"Start parsing {fileInfo.Name}"));

				var sharedValues = _localLogParserFactory().ParseLog(fileInfo.FullName);
				var herosLog = new EncounterLog(sharedValues.Target, null, fileInfo.FullName);
				_logFileConverter.ConvertLog(herosLog, sharedValues);
				_messageBus.SendMessage(new NewEncounterMessage(herosLog));
				_messageBus.SendMessage(new LogMessage($"Finished parsing {fileInfo.Name}"));

				_messageBus.SendMessage(new LogMessage($"Creating Html for {fileInfo.Name}"));
				Task.Run(() => _htmlFileWatcherFactory().CreateRaidHerosFile(herosLog));
			}
			catch (Exception exception)
			{
				_messageBus.SendMessage(new LogMessage(exception.ToString()));
			}
		}

		public void ParseLogFiles(IEnumerable<FileInfo> fileInfos)
		{
			LogfileWatcher.EnableRaisingEvents = false;
			foreach (var fileInfo in fileInfos)
			{
				ParseLogFile(fileInfo);
			}
			LogfileWatcher.EnableRaisingEvents = true;
		}

		private void LogFileOnCreated(object sender, FileSystemEventArgs e)
		{
			try
			{
				LogfileWatcher.EnableRaisingEvents = false;

				if (e.Name.EndsWith("tmp"))
				{
					return;
				}

				_messageBus.SendMessage(new LogMessage($"New log detected {e.Name}"));
				Thread.Sleep(int.Parse(Settings.Default.WaitTime));

				var fullPath = e.FullPath;

				var fileInfo = new FileInfo(fullPath);

				if (!fileInfo.Exists)
				{
					fullPath = fullPath + ".zip";
					fileInfo = new FileInfo(fullPath);
					if (!fileInfo.Exists)
					{
						_messageBus.SendMessage(new LogMessage($"Log vanished ?!"));
						return;
					}
				}

				ParseLogFile(fileInfo);
			}
			catch (Exception exception)
			{
				_messageBus.SendMessage(new LogMessage(exception.ToString()));
			}
			finally
			{
				LogfileWatcher.EnableRaisingEvents = true;
			}
		}
	}
}