using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using IniParser;
using Microsoft.Win32;
using RaidTool.Enums;
using RaidTool.Helper;
using RaidTool.Logic.Interfaces;
using RaidTool.Logic.LogDetectionStrategies;
using RaidTool.Messages;
using RaidTool.Models;
using RaidTool.Properties;
using ReactiveUI;

namespace RaidTool.ViewModels
{
	public class MainViewModel : ReactiveObject
	{
		private readonly IFileWatcher _fileWatcher;
		private readonly IMessageBus _messageBus;
		private readonly IEnumerable<ILogDetectionStrategy> _logDetectionStrategies;
		private bool _isVisible;
		private string _lastLogMessage;
		private LogFilterEnum _logFilter;
		private string _logType;
		private ObservableCollection<string> _parseMessages;
		private bool _raidHerosIsUpdating;
		private IEncounterLog _selectedLog;
		private bool Settin;

		public MainViewModel(IFileWatcher fileWatcher, IMessageBus messageBus, 
			IRaidHerosUpdater raidHerosUpdater, IEnumerable<ILogDetectionStrategy> logDetectionStrategies)
		{
			_fileWatcher = fileWatcher;
			_messageBus = messageBus;
			_logDetectionStrategies = logDetectionStrategies;

			_logDetectionStrategies.OrderBy(i => i.Name).ToList().ForEach(s => LogTypes.Add(s.Name));

			messageBus.Listen<NewEncounterMessage>().Subscribe(HandleNewEncounter);
			messageBus.Listen<UpdatedEncounterMessage>().Subscribe(HandleUpdatedEncounter);
			messageBus.Listen<LogMessage>().Subscribe(HandleNewLogMessage);

			var raidHerosUpdateTask = Task.Run(() =>
			{
				RaidHerosIsUpdating = true;
				raidHerosUpdater.UpdateRaidHeros();
			});

			var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			ParseMessages = new ObservableCollection<string>();
			OpenCommand = new RelayCommand(OpenLog, _ => SelectedLog != null);
			ClearCommand = new RelayCommand(ClearSelectedItem, _ => SelectedLog != null);
			ClearAllCommand = new RelayCommand(_ =>
			{
				RaidHerosLogFiles.Clear();
				DisplayedRaidHerosLogFiles.Clear();
			}, _ => RaidHerosLogFiles.Any());
			AddCommand = new RelayCommand(AddLog,
				_ => _fileWatcher.LogfileWatcher.EnableRaisingEvents && !RaidHerosIsUpdating);
			OpenFilePathCommand = new RelayCommand(_ => { OpenLogFilePath(directoryName); });

			_messageBus.SendMessage(new LogMessage("Waiting for new log."));

			LogFilter = (LogFilterEnum) int.Parse(Settings.Default.LogFilter);
			LogType = Settings.Default.LogType ?? LogTypes.First();

			_fileWatcher.Run();

			Task.Run(() =>
			{
				while (!raidHerosUpdateTask.IsCompleted)
				{
				}
				
				RaidHerosIsUpdating = false;
				_fileWatcher.LogfileWatcher.EnableRaisingEvents = true;
			});
		}

		public ObservableCollection<string> LogTypes { get; } = new ObservableCollection<string>();

		public bool RaidHerosIsUpdating
		{
			get => _raidHerosIsUpdating;
			set => _raidHerosIsUpdating = this.RaiseAndSetIfChanged(ref _raidHerosIsUpdating, value);
		}

		public string LogType
		{
			get => _logType;
			set
			{
				_logType = this.RaiseAndSetIfChanged(ref _logType, value);
				ChangeLogType();
			}
		}

		private void ChangeLogType()
		{
			var logDetectionStrategy = _logDetectionStrategies.First(s => s.Name.Contains(LogType));
			_fileWatcher.SetLogDetectionStrategy(logDetectionStrategy);
			Settings.Default.LogType = LogType;
			Settings.Default.Save();
		}

		public LogFilterEnum LogFilter
		{
			get => _logFilter;
			set
			{
				_logFilter = this.RaiseAndSetIfChanged(ref _logFilter, value);
				Settings.Default.LogFilter = ((int) _logFilter).ToString();
				Settings.Default.Save();
				OnLogFilterChanged();
			}
		}

		public ICommand OpenFilePathCommand { get; set; }

		public ICommand ClearAllCommand { get; set; }

		public ICommand ClearCommand { get; set; }

		public ObservableCollection<IEncounterLog> RaidHerosLogFiles { get; } = new ObservableCollection<IEncounterLog>();

		public IEncounterLog SelectedLog
		{
			get => _selectedLog;
			set
			{
				_selectedLog = this.RaiseAndSetIfChanged(ref _selectedLog, value);
				IsVisible = _selectedLog != null;
			}
		}

		public ICommand OpenCommand { get; set; }
		public ICommand AddCommand { get; set; }

		public bool IsVisible
		{
			get => _isVisible;
			set => _isVisible = this.RaiseAndSetIfChanged(ref _isVisible, value);
		}

		public string LastLogMessage
		{
			get => _lastLogMessage;
			set => _lastLogMessage = this.RaiseAndSetIfChanged(ref _lastLogMessage, value);
		}

		public ObservableCollection<string> ParseMessages
		{
			get => _parseMessages;
			set => _parseMessages = this.RaiseAndSetIfChanged(ref _parseMessages, value);
		}

		public ObservableCollection<IEncounterLog> DisplayedRaidHerosLogFiles { get; } = new ObservableCollection<IEncounterLog>();

		private void HandleUpdatedEncounter(UpdatedEncounterMessage encounterMessage)
		{
			_messageBus.SendMessage(
				new LogMessage($"Html available for {encounterMessage.EncounterLog.Name}. Waiting for new log."));

			if (bool.Parse(Settings.Default.OpenNewRaidHerosFiles) && File.Exists(encounterMessage.EncounterLog.ParsedLogPath))
			{
				Process.Start(encounterMessage.EncounterLog.ParsedLogPath);
			}
		}

		private void OpenLogFilePath(string directoryName)
		{
			var combine = Path.Combine(directoryName, $"{DateTime.Now.Date:yyyyMMdd}");
			if (Directory.Exists(combine))
			{
				Process.Start(combine);
			}
			else if (Directory.Exists(directoryName))
			{
				Process.Start(directoryName);
			}
			else
			{
				_messageBus.SendMessage(new LogMessage("Log file directory not found, open action canceled."));
			}
		}

		private void AddLog(object obj)
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = true;
			openFileDialog.Filter = "ArcDps log files (*.evtc, *.evtc.zip) | *.evtc;*.evtc.zip";

			if (openFileDialog.ShowDialog() == true)
			{
				var fileNames = openFileDialog.FileNames;

				try
				{
					var fileInfos = fileNames.Where(File.Exists).Select(i => new FileInfo(i));
					Task.Run(() => _fileWatcher.ParseLogFiles(fileInfos));
				}
				catch (Exception e)
				{
					_messageBus.SendMessage(new LogMessage(e.ToString()));
				}
			}
		}

		private void ClearSelectedItem(object obj)
		{
			if (RaidHerosLogFiles.Contains(SelectedLog))
			{
				RaidHerosLogFiles.Remove(SelectedLog);
			}
			if (DisplayedRaidHerosLogFiles.Contains(SelectedLog))
			{
				DisplayedRaidHerosLogFiles.Remove(SelectedLog);
			}
		}

		private void HandleNewLogMessage(LogMessage logMessage)
		{
			Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
				new Action(() =>
				{
					var collection = ParseMessages.Reverse().ToList();
					collection.Add(logMessage.Message);
					collection.Reverse();
					ParseMessages = new ObservableCollection<string>(collection);
					LastLogMessage = logMessage.Message;
				}));
		}

		private void OpenLog(object obj)
		{
			var encounterLog = obj as IEncounterLog;
			if (encounterLog != null)
			{
				if (File.Exists(encounterLog.ParsedLogPath))
				{
					Process.Start(encounterLog.ParsedLogPath);
				}
			}
			else
			{
				_messageBus.SendMessage(new LogMessage("html file not present, open action canceled."));
			}
		}

		public bool OpenNewRaidHerosFiles
		{
			get => bool.Parse(Settings.Default.OpenNewRaidHerosFiles);
			set
			{
				Settings.Default.OpenNewRaidHerosFiles = value.ToString();
				Settings.Default.Save();
			}
		}

		private void HandleNewEncounter(NewEncounterMessage encounterMessage)
		{
			var encounterLog = encounterMessage.EncounterLog;

			Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
				new Action(() =>
				{
					RaidHerosLogFiles.Add(encounterLog);
					FilterLogs(encounterLog);
					if (DisplayedRaidHerosLogFiles.Contains(encounterLog))
					{
						SelectedLog = encounterLog;
					}
				}));
		}

		private void FilterLogs(IEncounterLog encounterLog)
		{
			switch (LogFilter)
			{
				case LogFilterEnum.Latest:
					foreach (var oldLog in DisplayedRaidHerosLogFiles.Where(i => i.Name == encounterLog.Name).ToList())
					{
						DisplayedRaidHerosLogFiles.Remove(oldLog);
					}
					DisplayedRaidHerosLogFiles.Add(encounterLog);
					break;
				case LogFilterEnum.Succeeded:
					if (encounterLog.EncounterResult != "Fail")
					{
						DisplayedRaidHerosLogFiles.Add(encounterLog);
					}
					break;
				case LogFilterEnum.All:
				default:
					DisplayedRaidHerosLogFiles.Add(encounterLog);
					break;
			}
		}

		private void OnLogFilterChanged()
		{
			var selectedLog = SelectedLog;
			DisplayedRaidHerosLogFiles.Clear();

			switch (LogFilter)
			{
				case LogFilterEnum.Latest:
					foreach (var raidHerosLogs in RaidHerosLogFiles.OrderBy(i => i.EncounterDate).GroupBy(i => i.Name))
					{
						DisplayedRaidHerosLogFiles.Add(raidHerosLogs.Last());
					}
					break;
				case LogFilterEnum.Succeeded:
					foreach (var raidHerosLog in RaidHerosLogFiles.Where(i => i.EncounterResult != "Fail"))
					{
						DisplayedRaidHerosLogFiles.Add(raidHerosLog);
					}
					break;
				case LogFilterEnum.All:
				default:
					foreach (var raidHerosLogFile in RaidHerosLogFiles)
					{
						DisplayedRaidHerosLogFiles.Add(raidHerosLogFile);
					}
					break;
			}

			if (DisplayedRaidHerosLogFiles.Contains(selectedLog))
			{
				SelectedLog = selectedLog;
			}
		}
	}
}