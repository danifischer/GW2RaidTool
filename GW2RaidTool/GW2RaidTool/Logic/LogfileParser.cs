using System.Diagnostics;
using System.IO;
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

        public void ParseLogfile(string name, string path, string raidHerosPath)
        {
            if (path != null)
            {
                var raidHeroes = Path.Combine(raidHerosPath, "RaidHeros\\raid_heroes.exe");

                var processStartInfo = new ProcessStartInfo();
                processStartInfo.Arguments = '"' + path + '"';
                processStartInfo.FileName = raidHeroes;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.RedirectStandardInput = true;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.UseShellExecute = false;

                using (var process = Process.Start(processStartInfo))
                {
	                if (process != null && process.WaitForExit(30000))
	                {
		                _messageBus.SendMessage(new LogMessage($"html created for {name}"));
	                }
	                else
	                {
						_messageBus.SendMessage(new LogMessage($"Timeout while crating html for {name}"));
					}
                }
            }
        }
    }
}