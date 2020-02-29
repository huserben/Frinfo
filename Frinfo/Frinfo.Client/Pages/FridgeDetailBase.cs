using Frinfo.Client.Services;
using Frinfo.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frinfo.Client.Pages
{
   public class FridgeDetailBase : ComponentBase
   {
      [Inject]
      public IFridgeDataService FridgeDataService { get; set; }

      [Parameter]
      public string HouseholdId { get; set; }

      [Parameter]
      public string FridgeId { get; set; }

      public Fridge Fridge { get; private set; } = new Fridge();

      public List<FridgeItem> FridgeItems { get; } = new List<FridgeItem>();

      protected override async Task OnInitializedAsync()
      {
         Fridge = await FridgeDataService.GetFridgeById(int.Parse(HouseholdId), int.Parse(FridgeId));
         FridgeItems.AddRange(Fridge.Items);
      }

      protected async Task DeleteFridgeItem(FridgeItem fridgeItem)
      {
         var removedItem = await FridgeDataService.DeleteFridgeItem(Fridge.HouseholdId, Fridge.FridgeId, fridgeItem.FridgeItemId);

         if (removedItem)
         {
            FridgeItems.Remove(fridgeItem);
            StateHasChanged();
         }
      }
   }
}
