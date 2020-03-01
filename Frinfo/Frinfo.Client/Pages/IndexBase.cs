using Caliburn.Micro;
using Frinfo.Client.Events;
using Frinfo.Client.Services;
using Microsoft.AspNetCore.Components;
using System.Threading;
using System.Threading.Tasks;

namespace Frinfo.Client.Pages
{
   public class IndexBase : ComponentBase, IHandle<OnlineStateChangedEvent>
   {
      [Inject]
      public IEventAggregator EventAggregator { get; set; }

      [Inject]
      public IHttpClient FrinfoHttpClient { get; set; }

      public bool IsOnline { get; private set; }

      protected override void OnInitialized()
      {
         IsOnline = FrinfoHttpClient.IsOnline;
         EventAggregator.SubscribeOnPublishedThread(this);
      }

      public Task HandleAsync(OnlineStateChangedEvent message, CancellationToken cancellationToken)
      {
         IsOnline = message.IsOnline;
         StateHasChanged();

         return Task.CompletedTask;
      }
   }
}
