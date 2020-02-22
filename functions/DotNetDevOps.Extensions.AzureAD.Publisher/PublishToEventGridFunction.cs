using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace DotNetDevOps.Extensions.AzureAD.Publisher
{
    public class PublishToEventGridFunction
    {
        private readonly EventGridConfiguration options;

        public PublishToEventGridFunction(IOptions<EventGridConfiguration> options)
        {
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }
        [FunctionName(nameof(PublishToEventGridFunction))]
        public async Task PublishEvent(
        [QueueTrigger("azureadevents")] EventGridEvent @event,
        ILogger log)
        {

            string domainHostname = new Uri(options.Endpoint).Host;
            TopicCredentials domainKeyCredentials = new TopicCredentials(options.Key);
            EventGridClient client = new EventGridClient(domainKeyCredentials);

            @event.Topic = "mytest";
            await client.PublishEventsAsync(domainHostname, new List<EventGridEvent> {
              @event
            });

        }
        
    }
}
