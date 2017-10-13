using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using RaidTool.Logic.Interfaces;
using RaidTool.Messages;
using RaidTool.Models;
using ReactiveUI;

namespace RaidTool.Logic
{
	public class HtmlFileWatcher : IHtmlFileWatcher
	{
		private readonly ILogfileParser _logfileParser;
		private readonly IMessageBus _messageBus;
		private readonly IParsedFileCopier _parsedFileCopier;
		private readonly string _directoryName;
		private IEncounterLog _encounterLog;
		private FileSystemWatcher _fileSystemWatcher;

		public HtmlFileWatcher(IMessageBus messageBus, ILogfileParser logfileParser, IParsedFileCopier parsedFileCopier)
		{
			_messageBus = messageBus;
			_logfileParser = logfileParser;
			_parsedFileCopier = parsedFileCopier;
			_directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			StartWatcher();
		}

		public void CreateRaidHerosFile(IEncounterLog encounterLog)
		{
			_encounterLog = encounterLog;
		    _logfileParser.ParseLogfile(encounterLog.Name, encounterLog.EvtcPath, _directoryName);
		}

		private void StartWatcher()
		{
			_fileSystemWatcher = new FileSystemWatcher(_directoryName)
			{
				IncludeSubdirectories = false,
				Filter = "*.html"
			};
			_fileSystemWatcher.Created += ParsedLogOnCreated;
			_fileSystemWatcher.EnableRaisingEvents = true;
		}

		private void ParsedLogOnCreated(object sender, FileSystemEventArgs e)
		{
			var fileName = Path.GetFileName(_encounterLog.EvtcPath);
			if (fileName != null && fileName.Contains(e.Name.Split('.')[0]) == false)
			{
				return;	
			}

			try
			{
				_fileSystemWatcher.EnableRaisingEvents = false;
				_messageBus.SendMessage(new LogMessage("Moving Html file"));
				_encounterLog.ParsedLogPath = _parsedFileCopier.CopyFile(_directoryName, e.FullPath);
				_messageBus.SendMessage(new UpdatedEncounterMessage(_encounterLog));
			}
			catch (Exception exception)
			{
				_messageBus.SendMessage(new LogMessage(exception.ToString()));
			}
			finally
			{
				_fileSystemWatcher?.Dispose();
			}
		}
	}
}