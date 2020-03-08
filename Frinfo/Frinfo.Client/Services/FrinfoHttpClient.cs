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

      public FrinfoHttpClient(IEventAggregator eventAggregator)
      {
         httpClient = new HttpClient { BaseAddress = new Uri(ApiEndpoint) };
         SetHealthCheckTimer();

         this.eventAggregator = eventAggregator;
      }

      public bool IsOnline { get; private set; }

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

      public async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
      {
         await WaitForHealthCheckToFinish();

         if (IsOnline)
         {
            return await httpClient.PutAsync(requestUri, content);
         }

         return null;
      }

      private async Task WaitForHealthCheckToFinish()
      {
         while (IsCheckingOnlineStatus)
         {
            await Task.Delay(100);
         }
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
         var isFirstCheck = false;


         var isOnlineTask = UpdateOnlineStatus();

         if (healthTimer.Interval == 1)
         {
            healthTimer.Interval = 30000;
            healthTimer.AutoReset = true;
            isFirstCheck = true;
         }

         var currentOnlineState = await isOnlineTask;

         if (currentOnlineState != IsOnline || isFirstCheck)
         {
            IsOnline = currentOnlineState;
            await eventAggregator.PublishOnBackgroundThreadAsync(new OnlineStateChangedEvent(currentOnlineState));
         }

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
