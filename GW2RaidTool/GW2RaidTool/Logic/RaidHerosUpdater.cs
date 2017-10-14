using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using HtmlAgilityPack;
using RaidTool.Logic.Interfaces;
using RaidTool.Messages;
using RaidTool.Properties;
using ReactiveUI;

namespace RaidTool.Logic
{
	public class RaidHerosUpdater : IRaidHerosUpdater
	{
		private readonly IMessageBus _messageBus;
		private readonly string _httpsRaidheroesBaseUrl = "https://raidheroes.tk/";
		private string _archiveFileName = "rh.zip";

		public RaidHerosUpdater(IMessageBus messageBus)
		{
			_messageBus = messageBus;
		}

		public void UpdateRaidHeros()
		{
			var htmlAttribute = LoadVersionInformation();

			if (htmlAttribute != null)
			{
				if (htmlAttribute.Contains(Settings.Default.RaidHerosVerison))
				{
					_messageBus.SendMessage(new LogMessage("RaidHeros does not need an update."));
					return;
				}

				DownloadRaidHeros(htmlAttribute);

				var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				if (location == null)
				{
					_messageBus.SendMessage(new LogMessage("Could not resolve assembly location."));
					return;
				}

				var extractionPath = Path.Combine(location, "RaidHeros");

				CleanExtractionDirectory(extractionPath);

				ZipFile.ExtractToDirectory(_archiveFileName, extractionPath);
				File.Delete(_archiveFileName);
				Settings.Default.RaidHerosVerison = htmlAttribute.Replace("/", "").Replace(".zip", "");
				Settings.Default.Save();
				_messageBus.SendMessage(new LogMessage($"RaidHeros updated to {Settings.Default.RaidHerosVerison}"));
			}
		}

		private void DownloadRaidHeros(string htmlAttribute)
		{
			var concat = string.Concat(_httpsRaidheroesBaseUrl, htmlAttribute);
			using (var client = new WebClient())
			{
				client.DownloadFile(concat, _archiveFileName);
			}
		}

		private static void CleanExtractionDirectory(string extractionPath)
		{
			if (Directory.Exists(extractionPath))
			{
				var di = new DirectoryInfo(extractionPath);

				foreach (var file in di.GetFiles())
				{
					file.Delete();
				}
				foreach (var dir in di.GetDirectories())
				{
					dir.Delete(true);
				}
			}
			else
			{
				Directory.CreateDirectory(extractionPath);
			}
		}

		private string LoadVersionInformation()
		{
			try
			{
				var htmlWeb = new HtmlWeb();
				var htmlDocument = htmlWeb.Load(_httpsRaidheroesBaseUrl);

				var htmlNodes = htmlDocument.DocumentNode.Descendants("h3")
					.Where(i => i.InnerText == "Download").ToArray();

				var htmlAttribute = htmlNodes.First().ParentNode.Descendants("a").SingleOrDefault()?
					.Attributes.SingleOrDefault(i => i.Name == "href");

				return htmlAttribute?.Value;
			}
			catch (Exception e)
			{
				_messageBus.SendMessage(new LogMessage($"Error while determining RaidHeros version from raidheros.tk: {e.Message}"));
				return null;
			}
		}
	}
}