using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OpenHack
{
    public static class ProcessSalesEvents
    {
        [FunctionName("ProcessSalesEvents")]
        public static async Task Run([EventHubTrigger("oh4storeorders", Connection = "oh4eventhubns_SendPermissions_EVENTHUB")] EventData[] events,
                [CosmosDB(
                databaseName: "bfyoc",
                collectionName: "SalesEvents",
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<dynamic> saleseeventout,
            ILogger log)
        {
            var exceptions = new List<Exception>();

            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

                    dynamic jsonEventData = JsonConvert.DeserializeObject(messageBody);

                    jsonEventData.id = Guid.NewGuid().ToString();

                    await saleseeventout.AddAsync(jsonEventData);

                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
