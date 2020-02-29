using Blazored.LocalStorage;
using Frinfo.Client.Services;
using Frinfo.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frinfo.Client.Components
{
   public class HouseholdsBase : ComponentBase
   {
      private const string RecentHouseholdsKey = "RecentHouseholds";

      [Inject]
      public IHouseholdDataService HouseholdDataService { get; set; }

      [Inject]
      public ILocalStorageService LocalStorageService { get; set; }

      [Inject]
      public NavigationManager NavigationManager { get; set; }

      public string NewHouseholdName { get; set; }

      public string HouseholdCode { get; set; }

      public bool SearchedForCode { get; set; }

      public bool SearchSuccessful { get; set; }

      public List<Household> RecentHouseholds { get; } = new List<Household>();

      protected override async Task OnInitializedAsync()
      {
         await FetchRecentHouseholds();
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
            RecentHouseholds.Insert(0, newHousehold);
            await UpdateRecentList();

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
            await UpdateRecentList();
         }

         StateHasChanged();
      }

      protected void NavigateToHousehold(int householdId)
      {
         NavigationManager.NavigateTo($"household/{householdId}");
      }

      protected async Task DeleteHousehold(Household household)
      {
         RecentHouseholds.Remove(household);
         var deleteSuccessful = await HouseholdDataService.DeleteHousehold(household.HouseholdId);

         if (deleteSuccessful)
         {
            await UpdateRecentList();
         }

         StateHasChanged();
      }

      protected async Task RemoveFromRecentList(Household household)
      {
         RecentHouseholds.Remove(household);
         await UpdateRecentList();

         StateHasChanged();
      }

      private async Task UpdateRecentList()
      {
         var recentHouseholdIds = RecentHouseholds.Select(h => h.HouseholdId).ToList();
         await LocalStorageService.SetItemAsync(RecentHouseholdsKey, recentHouseholdIds);
      }

      private async Task FetchRecentHouseholds()
      {
         var containsFavoriteHouseholds = await LocalStorageService.ContainKeyAsync(RecentHouseholdsKey);
         if (!containsFavoriteHouseholds)
         {
            await LocalStorageService.SetItemAsync(RecentHouseholdsKey, new List<int>());
         }
         else
         {
            var recentHouseholds = await LocalStorageService.GetItemAsync<List<int>>(RecentHouseholdsKey);
            foreach (var householdId in recentHouseholds)
            {
               var recentHousehold = await HouseholdDataService.GetHouseholdById(householdId);

               if (recentHousehold != null)
               {
                  RecentHouseholds.Add(recentHousehold);
               }
            }
         }
      }
   }
}
