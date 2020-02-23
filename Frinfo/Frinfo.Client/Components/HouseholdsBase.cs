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

      public List<Household> Households { get; set; } = new List<Household>();

      protected override async Task OnInitializedAsync()
      {
         var households = await HouseholdDataService.GetAllHouseholds();
         Households.AddRange(households);
      }
   }
}
