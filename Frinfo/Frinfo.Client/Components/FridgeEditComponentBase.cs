using Blazor.FileReader;
using Frinfo.Client.Services;
using Frinfo.Shared;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Frinfo.Client.Components
{
   public class FridgeEditComponentBase : ComponentBase
   {
      public bool ShowDialog { get; set; }

      public Fridge Fridge { get; set; }

      [Parameter]
      public EventCallback<bool> CloseEventCallback { get; set; }

      [Parameter]
      public Household Household { get; set; }

      [Inject]
      public IFridgeDataService FridgeDataService { get; set; }

      public string Title
      {
         get
         {
            if (Fridge.FridgeId == 0)
            {
               return "Add new Fridge";
            }

            return $"Edit {Fridge.Name}";
         }
      }

      public void Show()
      {
         ShowDialog = true;
         StateHasChanged();
      }

      protected override void OnInitialized()
      {
         if (Fridge == null)
         {
            Fridge = new Fridge();
         }
      }

      public void Close()
      {
         ShowDialog = false;
         StateHasChanged();
      }

      protected async Task AddFridge()
      {
         Fridge.HouseholdId = Household.HouseholdId;

         if (Fridge.FridgeId == 0)
         {
            Fridge = await FridgeDataService.AddNewFridge(Fridge);
         }
         else
         {
            await FridgeDataService.UpdateFridge(Fridge);
         }

         ShowDialog = false;
         await CloseEventCallback.InvokeAsync(true);
         StateHasChanged();
      }

      protected void OnCancelEditItem()
      {
         ShowDialog = false;
         Fridge = new Fridge();
         StateHasChanged();
      }
   }
}
