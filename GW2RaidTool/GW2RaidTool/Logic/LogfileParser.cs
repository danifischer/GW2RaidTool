using System.Diagnostics;
using System.IO;
using System.Reflection;
using RaidTool.Logic.Interfaces;
using RaidTool.Messages;
using ReactiveUI;

namespace RaidTool.Logic
{
	public class LogfileParser : ILogfileParser
	{
		private readonly IMessageBus _messageBus;

		public LogfileParser(IMessageBus messageBus)
		{
			_messageBus = messageBus;
		}

        public bool ParseLogfile(string name, string evtcPath, string outputDirectory, string raidHerosPath)
        {
            if (evtcPath != null)
            {
                var raidHeroes = Path.Combine(raidHerosPath, "RaidHeros\\raid_heroes.exe");

                var processStartInfo = new ProcessStartInfo();
                processStartInfo.Arguments = '"' + evtcPath + '"';
                processStartInfo.FileName = raidHeroes;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.RedirectStandardInput = true;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.UseShellExecute = false;
				processStartInfo.WorkingDirectory = Path.Combine(outputDirectory);

				using (var process = Process.Start(processStartInfo))
                {
	                if (process != null && process.WaitForExit(30000))
	                {
		                _messageBus.SendMessage(new LogMessage($"html created for {name}"));
		                return true;
	                }

					_messageBus.SendMessage(new LogMessage($"Timeout while crating html for {name}"));
                }
            }

	        return false;
        }
    }
}