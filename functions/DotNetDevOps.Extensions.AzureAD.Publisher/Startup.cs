using DotNetDevOps.Extensions.AzureAD.Publisher;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]

namespace DotNetDevOps.Extensions.AzureAD.Publisher
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddHttpClient<GraphClient>();

            builder.Services.AddOptions<EventGridConfiguration>().Configure<IConfiguration>((o, c) =>
            {
                c.GetSection("EventGrid").Bind(o);
            });
        }
    }
}
