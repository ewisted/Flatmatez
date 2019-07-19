using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DALFunctions
{
	public static class SaveBill
	{
		[FunctionName("SaveBill")]
		public static void Run([ServiceBusTrigger("sendbill", "dbUpdate", Connection = "Endpoint=sb://flatmatezeb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=+nN0Ph/0nEmEIBptFMc2wcgt8AQkWAKel/VqoWeGJR4=")]string billJson, ILogger log)
		{
			log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
		}
	}
}
