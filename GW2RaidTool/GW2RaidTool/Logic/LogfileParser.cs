using System.Diagnostics;
using System.IO;
using RaidTool.Logic.Interfaces;

namespace RaidTool.Logic
{
	public class LogfileParser : ILogfileParser
	{
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
	                process?.WaitForExit();
                }
            }
        }
    }
}