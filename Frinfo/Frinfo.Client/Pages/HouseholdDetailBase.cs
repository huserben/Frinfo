using Frinfo.Client.Services;
using Frinfo.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frinfo.Client.Pages
{
   public class HouseholdDetailBase : ComponentBase
   {
      [Inject]
      public IHouseholdDataService HouseholdDataService { get; set; }

      [Inject]
      public IFridgeDataService FridgeDataService { get; set; }

      [Inject]
      public NavigationManager NavigationManager { get; set; }

      [Parameter]
      public string HouseholdId { get; set; }

      public Household Household { get; private set; } = new Household();

      public string NewFridgeName { get; set; }

      public List<Fridge> Fridges { get; } = new List<Fridge>();

      protected override async Task OnInitializedAsync()
      {
         Household = await HouseholdDataService.GetHouseholdById(int.Parse(HouseholdId));
         Fridges.AddRange(Household.Fridges);
      }

      protected void NavigateToFridge(int fridgeId)
      {
         NavigationManager.NavigateTo($"household/{HouseholdId}/fridge/{fridgeId}");
      }

      protected async Task DeleteFridge(Fridge fridge)
      {
         var removedFridge = await FridgeDataService.DeleteFridge(Household.HouseholdId, fridge.FridgeId);

         if (removedFridge)
         {
            Fridges.Remove(fridge);
            StateHasChanged();
         }
      }

      protected async Task OnAddNewFridge()
      {
         var newFridge = await FridgeDataService.AddNewFridge(Household.HouseholdId, NewFridgeName);
         if (newFridge != null)
         {
            NavigateToFridge(newFridge.FridgeId);
         }
      }
   }
}
