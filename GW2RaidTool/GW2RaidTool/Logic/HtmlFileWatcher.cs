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
		private readonly string _directoryName;
		private IEncounterLog _encounterLog;
		private FileSystemWatcher _fileSystemWatcher;

		public HtmlFileWatcher(IMessageBus messageBus, ILogfileParser logfileParser)
		{
			_messageBus = messageBus;
			_logfileParser = logfileParser;
			_directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			StartWatcher();
		}

		public void CreateRaidHerosFile(IEncounterLog encounterLog)
		{
			_encounterLog = encounterLog;
		    _logfileParser.ParseLogfile(encounterLog.Name, encounterLog.EvtcPath, GetOutputDir(_directoryName), _directoryName);
		}

		private void StartWatcher()
		{
			_fileSystemWatcher = new FileSystemWatcher(_directoryName)
			{
				IncludeSubdirectories = true,
				Filter = "*.html"
			};
			_fileSystemWatcher.Changed += ParsedLogOnCreated;
			_fileSystemWatcher.EnableRaisingEvents = true;
		}

		public string GetOutputDir(string path)
		{
			var combine = Path.Combine(path, $"{DateTime.Now.Date:yyyyMMdd}");
			
			if (!Directory.Exists(combine))
			{
				Directory.CreateDirectory(combine);
			}

			return combine;
		}

		private void ParsedLogOnCreated(object sender, FileSystemEventArgs e)
		{
			var fileName = Path.GetFileName(_encounterLog.EvtcPath)?.Split('.')[0];
			if (fileName != null && e.Name.Contains(fileName) == false)
			{
				return;	
			}

			try
			{
				_fileSystemWatcher.EnableRaisingEvents = false;
				_encounterLog.ParsedLogPath = e.FullPath;
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