using Blazor.FileReader;
using Frinfo.Client.Services;
using Frinfo.Shared;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Frinfo.Client.Components
{
   public class HouseholdEditComponentBase : ComponentBase
   {
      public bool ShowDialog { get; set; }

      public Household Household { get; set; } = new Household();

      [Parameter]
      public EventCallback<bool> CloseEventCallback { get; set; }

      [Inject]
      public IHouseholdDataService HouseholdDataService { get; set; }

      public string Title
      {
         get
         {
            if (Household.HouseholdId == 0)
            {
               return "Add new Household";
            }

            return $"Edit {Household.Name}";
         }
      }

      public void Show()
      {
         ShowDialog = true;
         StateHasChanged();
      }

      public void Close()
      {
         ShowDialog = false;
         StateHasChanged();
      }

      protected async Task AddHousehold()
      {
         if (Household.HouseholdId == 0)
         {
            Household = await HouseholdDataService.AddNewHousehold(Household.Name);
         }
         else
         {
            await HouseholdDataService.UpdateHousehold(Household);
         }

         ShowDialog = false;
         await CloseEventCallback.InvokeAsync(true);
         StateHasChanged();
      }

      protected void OnCancelEditItem()
      {
         ShowDialog = false;
         Household = new Household();
         StateHasChanged();
      }
   }
}
