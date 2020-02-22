using DotNetDevOps.Extensions.AzureAD.Publisher;
using Microsoft.Azure.WebJobs.Hosting;

 

namespace DotNetDevOps.Extensions.AzureAD.Publisher
{
    public class EventGridConfiguration
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
        public string ResourceId { get; set; }
    }
}
