using Caliburn.Micro;
using Frinfo.Client.Events;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace Frinfo.Client.Services
{
   public class FrinfoHttpClient : IHttpClient
   {
      private readonly HttpClient httpClient;
      private readonly IEventAggregator eventAggregator;
      private Timer healthTimer;
      private bool isOnline;

      public FrinfoHttpClient(IEventAggregator eventAggregator)
      {
         httpClient = new HttpClient { BaseAddress = new Uri(ApiEndpoint) };
         SetHealthCheckTimer();

         this.eventAggregator = eventAggregator;
      }

      public bool IsOnline
      {
         get
         {
            return isOnline;
         }

         private set
         {
            if (value != isOnline)
            {
               isOnline = value;
               eventAggregator.PublishOnBackgroundThreadAsync(new OnlineStateChangedEvent(value));
            }
         }
      }

      private string ApiEndpoint
      {
         get { return "https://localhost:44304"; }
      }

      private bool IsCheckingOnlineStatus { get; set; }

      public async Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption)
      {
         await WaitForHealthCheckToFinish();

         if (IsOnline)
         {
            return await httpClient.GetAsync(requestUri, completionOption);
         }

         return null;
      }

      public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
      {
         await WaitForHealthCheckToFinish();

         if (IsOnline)
         {
            return await httpClient.DeleteAsync(requestUri);
         }

         return null;
      }

      public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
      {
         await WaitForHealthCheckToFinish();

         if (IsOnline)
         {
            return await httpClient.PostAsync(requestUri, content);
         }

         return null;
      }

      private async Task WaitForHealthCheckToFinish()
      {
         while (IsCheckingOnlineStatus)
         {
            await Task.Delay(100);
         }

         return;
      }

      private void SetHealthCheckTimer()
      {
         IsCheckingOnlineStatus = true;
         healthTimer = new Timer(1);
         healthTimer.Elapsed += OnCheckHealth;
         healthTimer.AutoReset = false;
         healthTimer.Enabled = true;
      }

      private async void OnCheckHealth(object sender, ElapsedEventArgs e)
      {
         IsCheckingOnlineStatus = true;
         if (healthTimer.Interval == 1)
         {
            healthTimer.Interval = 30000;
            healthTimer.AutoReset = true;
         }

         var isOnline = await UpdateOnlineStatus();
         IsOnline = isOnline;
         IsCheckingOnlineStatus = false;
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
