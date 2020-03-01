using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace Frinfo.Client.Services
{
   public class FrinfoHttpClient : IHttpClient
   {
      private readonly HttpClient httpClient;
      private Timer healthTimer;

      public FrinfoHttpClient()
      {
         httpClient = new HttpClient { BaseAddress = new Uri(ApiEndpoint) };

         SetHealthCheckTimer();
      }

      private string ApiEndpoint
      {
         get { return "https://localhost:44304"; }
      }

      public bool IsOnline
      {
         get;
         private set;
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
      private void SetHealthCheckTimer()
      {
         healthTimer = new Timer(1);
         healthTimer.Elapsed += OnCheckHealth;
         healthTimer.AutoReset = false;
         healthTimer.Enabled = true;
      }

      private async void OnCheckHealth(object sender, ElapsedEventArgs e)
      {
         if (healthTimer.Interval == 1)
         {
            healthTimer.Interval = 30000;
            healthTimer.AutoReset = true;
         }

         var isOnline = await UpdateOnlineStatus();
         IsOnline = isOnline;
      }

      private async Task<bool> UpdateOnlineStatus()
      {
         try
         {
            var response = await httpClient.GetAsync("health");
            return response.IsSuccessStatusCode;
         }
         catch
         {
            return false;
         }
      }
   }
}
