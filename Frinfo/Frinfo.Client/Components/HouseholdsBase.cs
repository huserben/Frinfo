using Caliburn.Micro;
using Frinfo.Client.Services;
using Frinfo.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frinfo.Client.Events;
using System.Threading;
using Blazored.Toast.Services;

namespace Frinfo.Client.Components
{
   public class HouseholdsBase : ComponentBase, IHandle<OnlineStateChangedEvent>
   {
      [Inject]
      public IHouseholdDataService HouseholdDataService { get; set; }

      [Inject]
      public ILocalStorageHouseholdService LocalStorageHouseholdService { get; set; }

      [Inject]
      public NavigationManager NavigationManager { get; set; }

      [Inject]
      public IEventAggregator EventAggregator { get; set; }

      [Inject]
      public IHttpClient FrinfoHttpClient { get; set; }

      [Inject]
      public IToastService ToastService { get; set; }

      public string NewHouseholdName { get; set; }

      public string HouseholdCode { get; set; }

      public bool IsOffline { get; private set; } = false;

      public List<Household> RecentHouseholds { get; } = new List<Household>();

      protected HouseholdEditComponent EditHousehold { get; set; }

      public Task HandleAsync(OnlineStateChangedEvent message, CancellationToken cancellationToken)
      {
         IsOffline = !message.IsOnline;
         StateHasChanged();

         return Task.CompletedTask;
      }

      protected override async Task OnInitializedAsync()
      {
         await FetchRecentHouseholds();

         IsOffline = !FrinfoHttpClient.IsOnline;
         EventAggregator.SubscribeOnBackgroundThread(this);

         StateHasChanged();
      }

      protected async Task OnSearchForCode()
      {
         var alreadyAddedHousehold = RecentHouseholds.FirstOrDefault(h => h.HouseholdCode == HouseholdCode);
         if (alreadyAddedHousehold != null)
         {
            NavigateToHousehold(alreadyAddedHousehold.HouseholdId);
         }

         var household = await HouseholdDataService.GetHouseholdByCode(HouseholdCode);
         
         var searchSuccessful = household != null;

         if (searchSuccessful)
         {
            RecentHouseholds.Insert(0, household);
            NavigateToHousehold(household.HouseholdId);
         }
         else
         {
            ToastService.ShowError($"Could not find any household with code {HouseholdCode}");
         }
      }

      protected void NavigateToHousehold(int householdId)
      {
         NavigationManager.NavigateTo($"household/{householdId}");
      }

      protected async Task DeleteHousehold(Household household)
      {
         var deleteSuccessful = await HouseholdDataService.DeleteHousehold(household.HouseholdId);

         if (deleteSuccessful)
         {
            await RemoveFromRecentList(household);
         }
      }

      protected async Task RemoveFromRecentList(Household household)
      {
         RecentHouseholds.Remove(household);
         await LocalStorageHouseholdService.RemoveHousehold(household.HouseholdId);

         StateHasChanged();
      }

      protected void OnAddHousehold()
      {
         EditHousehold.Household = new Household();
         EditHousehold.Show();
      }

      protected void EditHousehold_OnClose()
      {
         if (EditHousehold.Household != null)
         {
            NavigateToHousehold(EditHousehold.Household.HouseholdId);
         }
      }

      private async Task FetchRecentHouseholds()
      {
         var households = await LocalStorageHouseholdService.GetLocallyStoredHouseholds();
         foreach (var household in households)
         {
            RecentHouseholds.Add(household);
         }
      }
   }
}
