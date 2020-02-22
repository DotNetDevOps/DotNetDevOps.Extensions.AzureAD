using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetDevOps.Extensions.AzureAD.Publisher;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;


namespace DotNetDevOps.Extensions.AzureAD.Publisher
{

    public class SyncAzureADFunction
    {
        private readonly GraphClient graphClient;
        public SyncAzureADFunction(GraphClient graph)
        {
            this.graphClient = graph;
        }

     
       
        [FunctionName(nameof(SyncAzureADFunction))]
        public  async Task SyncAzureADChecker(
            [Blob("azureadpublisher/state.json")] CloudBlockBlob blob,
            [TimerTrigger("0 */5 * * * *", RunOnStartup =true)]TimerInfo myTimer,
            [Queue("azureadevents")] ICollector<EventGridEvent> events,
            ILogger log)
        {
             
            var a = await this.graphClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "v1.0/users"));
            var s = Newtonsoft.Json.Linq.JToken.Parse(await a.Content.ReadAsStringAsync());
            var dictionary = new Dictionary<string, string>();


            if(await blob.ExistsAsync())
            {
                dictionary = CustomSerializer.Deserialize<Dictionary<string, string>>(await blob.DownloadTextAsync());
            }

            var removedCheck = dictionary.Keys.ToHashSet();
            foreach (var user in s.SelectToken("$.value"))
            {
                var id = user.SelectToken("$.id").ToString();
                var idHash=id.AsHash(); 
                var objHash = user.ToString().AsHash();
                
               
                if (dictionary.ContainsKey(idHash))
                { //Existing User

                    removedCheck.Remove(id);

                    if (dictionary[idHash] != objHash)
                    { //Updated existing User 

                        dictionary[idHash] = objHash;

                        events.Add(new EventGridEvent
                        {
                            Id = objHash,
                            DataVersion = "1.0",
                            Data = user,
                            EventTime = DateTime.UtcNow,
                            EventType = "AzureAD.UpdatedUser",
                            Subject = "AzureAD.UpdatedUser/" + id
                        });
                    }
                }
                else 
                { //New User
                    events.Add(new EventGridEvent
                    {
                        Id = objHash,
                        DataVersion= "1.0",
                        Data = user,
                        EventTime = DateTime.UtcNow,
                        EventType ="AzureAD.NewUser",
                        Subject ="AzureAD.NewUser/" + id 
                    });
                }
            }

            foreach(var id in removedCheck)
            {
                events.Add(new EventGridEvent
                {
                    Id = id.AsHash(),
                    DataVersion = "1.0",
                    Data = s.SelectToken($"$.value[?(@.id == '{id}')]"),
                    EventTime = DateTime.UtcNow,
                    EventType = "AzureAD.RemovedUser",
                    Subject = "AzureAD.RemovedUser/" + id
                });
            }
            var bytes = CustomSerializer.SerializeToUtf8Bytes(dictionary);
            await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
            //https://graph.microsoft.com/

        }
    }
}
