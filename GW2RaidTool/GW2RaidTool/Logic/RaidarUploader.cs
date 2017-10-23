using System.Security.Cryptography;
using System.Text;
using RaidTool.Helper;
using RaidTool.Logic.Interfaces;
using RaidTool.Messages;
using RaidTool.Models;
using RaidTool.Properties;
using ReactiveUI;
using RestSharp;

namespace RaidTool.Logic
{
    public class RaidarUploader : IRaidarUploader
    {
	    private readonly IMessageBus _messageBus;

	    public RaidarUploader(IMessageBus messageBus)
	    {
		    _messageBus = messageBus;
	    }

	    public void Upload(IEncounterLog encounterLog)
	    {
		    var restClient = new RestClient("https://www.gw2raidar.com");
		    var restRequest = new RestRequest("/api/upload.json");
			restRequest.Method = Method.POST;
		    restRequest.AddFile("file", encounterLog.EvtcPath);
		    restRequest.AddParameter("username", Settings.Default.RaidarUser);
			restRequest.AddParameter("password", Settings.Default.RaidarPassword.Unprotect());

		    var restResponse = restClient.Execute(restRequest);

		    if (restResponse.ResponseStatus == ResponseStatus.Completed)
		    {
			    encounterLog.UploadComplete = true;
			    _messageBus.SendMessage(new UploadedEncounterMessage(encounterLog));
			}
		    else
		    {
			    encounterLog.UploadComplete = false;
				_messageBus.SendMessage(new LogMessage(restResponse.ErrorMessage));
			}

			
	    }
    }
}
