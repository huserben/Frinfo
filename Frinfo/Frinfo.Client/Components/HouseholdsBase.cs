using Caliburn.Micro;
using Frinfo.Client.Services;
using Frinfo.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frinfo.Client.Events;
using System.Threading;

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

      public string NewHouseholdName { get; set; }

      public string HouseholdCode { get; set; }

      public bool SearchedForCode { get; set; }

      public bool SearchSuccessful { get; set; }

      public bool IsOffline { get; private set; } = false;

      public List<Household> RecentHouseholds { get; } = new List<Household>();

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

      protected async Task OnAddNewHousehold()
      {
         if (string.IsNullOrEmpty(NewHouseholdName))
         {
            return;
         }

         var newHousehold = await HouseholdDataService.AddNewHousehold(NewHouseholdName);

         if (newHousehold != null)
         {
            NavigateToHousehold(newHousehold.HouseholdId);
         }
      }

      protected async Task OnSearchForCode()
      {
         if (RecentHouseholds.Any(h => h.HouseholdCode == HouseholdCode))
         {
            return;
         }

         var household = await HouseholdDataService.GetHouseholdByCode(HouseholdCode);
         SearchedForCode = true;
         SearchSuccessful = household != null;

         if (SearchSuccessful)
         {
            RecentHouseholds.Insert(0, household);
         }

         StateHasChanged();
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
