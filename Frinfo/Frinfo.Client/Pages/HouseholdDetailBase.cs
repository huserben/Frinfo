using Blazored.Toast.Services;
using Caliburn.Micro;
using Frinfo.Client.Components;
using Frinfo.Client.Events;
using Frinfo.Client.Services;
using Frinfo.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Frinfo.Client.Pages
{
   public class HouseholdDetailBase : ComponentBase, IHandle<OnlineStateChangedEvent>
   {
      [Inject]
      public IHouseholdDataService HouseholdDataService { get; set; }

      [Inject]
      public IFridgeDataService FridgeDataService { get; set; }

      [Inject]
      public IEventAggregator EventAggregator { get; set; }

      [Inject]
      public IHttpClient FrinfoHttpClient { get; set; }

      [Inject]
      public IToastService ToastService { get; set; }

      [Inject]
      public NavigationManager NavigationManager { get; set; }

      [Parameter]
      public string HouseholdId { get; set; }

      public Household Household { get; private set; } = new Household();

      public List<Fridge> Fridges { get; } = new List<Fridge>();

      public bool IsOffline { get; set; }

      protected FridgeEditComponent EditFridge { get; set; }

      protected HouseholdEditComponent EditHousehold { get; set; }

      public Task HandleAsync(OnlineStateChangedEvent message, CancellationToken cancellationToken)
      {
         IsOffline = !message.IsOnline;

         return Task.CompletedTask;
      }

      protected override async Task OnInitializedAsync()
      {
         Household = await HouseholdDataService.GetHouseholdById(int.Parse(HouseholdId));
         Fridges.AddRange(Household.Fridges);

         IsOffline = !FrinfoHttpClient.IsOnline;

         EventAggregator.SubscribeOnPublishedThread(this);
      }

      protected void NavigateToFridge(int fridgeId)
      {
         NavigationManager.NavigateTo($"household/{HouseholdId}/fridge/{fridgeId}");
      }

      protected async Task DeleteFridge(Fridge fridge)
      {
         var wasRemoveSuccessfull = await FridgeDataService.DeleteFridge(Household.HouseholdId, fridge.FridgeId);

         if (wasRemoveSuccessfull)
         {
            Fridges.Remove(fridge);
            StateHasChanged();

            ToastService.ShowInfo($"Removed {fridge.Name}");
         }
         else
         {
            ToastService.ShowError($"Failed to remove {fridge.Name}", "Delete Failed");
         }
      }

      protected async Task OnRemoveHousehold()
      {
         var removeSuccessful = await HouseholdDataService.DeleteHousehold(Household.HouseholdId);
         if (removeSuccessful)
         {
            NavigationManager.NavigateTo("/");
            ToastService.ShowInfo($"Successfully removed {Household.Name}");
         }
         else
         {
            ToastService.ShowError($"Could not remove {Household.Name}", "Delete failed");
         }
      }

      protected void OnEditHousehold()
      {
         EditHousehold.Household = Household;
         EditHousehold.Show();
      }

      protected async Task EditHousehold_OnClose()
      {
         Household = await HouseholdDataService.GetHouseholdById(int.Parse(HouseholdId));
         StateHasChanged();
      }

      protected void AddFridge_OnClose()
      {
         var newFridge = EditFridge.Fridge;
         NavigateToFridge(newFridge.FridgeId);
      }

      protected void AddNewFridge()
      {
         EditFridge.Fridge = new Fridge();
         EditFridge.Show();
      }
   }
}
