using System;
using Autofac;
using EVTC_Log_Parser;
using RaidTool.Logic;
using RaidTool.Logic.Interfaces;
using RaidTool.ViewModels;
using ReactiveUI;

namespace RaidTool
{
	public static class BootStrapper
	{
		[STAThread]
		public static void Main(string[] args)
		{
			var containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterType<MessageBus>().As<IMessageBus>().SingleInstance();
			containerBuilder.RegisterType<LogfileParser>().As<ILogfileParser>();
			containerBuilder.RegisterType<LogFileConverter>().As<ILogFileConverter>();
			containerBuilder.RegisterType<ParsedFileCopier>().As<IParsedFileCopier>();
			containerBuilder.RegisterType<FileWatcher>().As<IFileWatcher>();
			containerBuilder.RegisterType<HtmlFileWatcher>().As<IHtmlFileWatcher>();
			containerBuilder.RegisterType<LocalLogParser>().As<ILocalLogParser>();
			containerBuilder.RegisterType<RaidHerosUpdater>().As<IRaidHerosUpdater>();
			containerBuilder.RegisterType<MainWindow>();
			containerBuilder.RegisterType<MainViewModel>();

			var container = containerBuilder.Build();
			var app = new App();
			app.InitializeComponent();
			var mainWindow = container.Resolve<MainWindow>();
			app.Run(mainWindow);
		}
	}
}
