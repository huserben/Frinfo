using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Frinfo.Client.Services
{
   public class FrinfoHttpClient : IHttpClient
   {
      private readonly HttpClient httpClient;

      public FrinfoHttpClient()
      {
         httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:44304") };
      }

      public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption)
      {
         return httpClient.GetAsync(requestUri, completionOption);
      }

      public Task<HttpResponseMessage> DeleteAsync(string requestUri)
      {
         return httpClient.DeleteAsync(requestUri);
      }

      public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
      {
         return httpClient.PostAsync(requestUri, content);
      }
   }
}
