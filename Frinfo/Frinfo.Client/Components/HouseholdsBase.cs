using Frinfo.Client.Services;
using Frinfo.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frinfo.Client.Components
{
   public class HouseholdsBase : ComponentBase
   {
      [Inject]
      public IHouseholdDataService HouseholdDataService { get; set; }

      public Household Household { get; set; }

      public string HouseholdCode { get; set; }

      public bool SearchedForCode { get; set; }

      protected async Task HandleSubmit()
      {
         Household = await HouseholdDataService.GetHouseholdByCode(HouseholdCode);
         SearchedForCode = true;
         StateHasChanged();
      }
   }
}
