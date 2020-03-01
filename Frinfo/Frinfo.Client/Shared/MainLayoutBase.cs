using Blazored.Toast.Services;
using Caliburn.Micro;
using Frinfo.Client.Events;
using Frinfo.Client.Services;
using Microsoft.AspNetCore.Components;
using System.Threading;
using System.Threading.Tasks;

namespace Frinfo.Client.Shared
{
   public class MainLayoutBase : LayoutComponentBase, IHandle<OnlineStateChangedEvent>
   {
      [Inject]
      public IEventAggregator EventAggregator { get; set; }

      [Inject]
      public IHttpClient FrinfoHttpClient { get; set; }

      [Inject]
      public IToastService ToastService { get; set; }

      public bool IsOnline { get; private set; }

      protected override void OnInitialized()
      {
         IsOnline = FrinfoHttpClient.IsOnline;
         EventAggregator.SubscribeOnUIThread(this);
      }

      public Task HandleAsync(OnlineStateChangedEvent message, CancellationToken cancellationToken)
      {
         IsOnline = message.IsOnline;
         StateHasChanged();

         if (IsOnline)
         {
            ToastService.ShowSuccess("Connection to Backend Established", "Online");
         }
         else
         {
            ToastService.ShowInfo("No Connection to Backend possible", "Offline");
         }

         return Task.CompletedTask;
      }
   }
}
