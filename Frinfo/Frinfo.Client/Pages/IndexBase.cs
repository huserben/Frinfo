using Caliburn.Micro;
using Frinfo.Client.Events;
using Microsoft.AspNetCore.Components;
using System.Threading;
using System.Threading.Tasks;

namespace Frinfo.Client.Pages
{
   public class IndexBase : ComponentBase, IHandle<OnlineStateChangedEvent>
   {
      [Inject]
      public IEventAggregator EventAggregator { get; set; }

      public bool IsOnline { get; private set; }

      protected override void OnInitialized()
      {
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
