using System;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetDevOps.Extensions.AzureAD.Publisher;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs.Hosting;

 

namespace DotNetDevOps.Extensions.AzureAD.Publisher
{
    public class GraphClient
    {
        private readonly HttpClient httpClient;

        public GraphClient(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            httpClient.BaseAddress = new Uri("https://graph.microsoft.com/");
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {


            var tokenProvider = new AzureServiceTokenProvider();

            var token = tokenProvider.GetAccessTokenAsync("https://graph.microsoft.com/").Result;

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return httpClient.SendAsync(request);
        }
    }
}
