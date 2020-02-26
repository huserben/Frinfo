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

      [Parameter]
      public string HouseholdId { get; set; }

      public Household Household { get; private set; } = new Household();

      protected override async Task OnInitializedAsync()
      {
         Household = await HouseholdDataService.GetHouseholdById(int.Parse(HouseholdId));
      }
   }
}
