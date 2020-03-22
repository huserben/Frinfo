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
using System;

namespace Frinfo.Client.Components
{
   public class DueFoodBase : ComponentBase
   {
      [Inject]
      public ILocalStorageHouseholdService LocalStorageHouseholdService { get; set; }

      [Inject]
      public NavigationManager NavigationManager { get; set; }

      public List<FridgeItem> ItemsAboutToExpire { get; set; } = new List<FridgeItem>();

      [Parameter]
      public int? HouseholdId { get; set; }

      [Parameter]
      public int? FridgeId { get; set; }

      protected override async Task OnInitializedAsync()
      {
         var fridgeItems = await LocalStorageHouseholdService.GetLocallyStoredFridgeIems(HouseholdId, FridgeId);

         var aboutToExpire = fridgeItems.Where(x => x.ExpirationDate.HasValue).OrderBy(x => x.ExpirationDate).Take(5);

         ItemsAboutToExpire.AddRange(aboutToExpire);

         StateHasChanged();
      }

      protected string GetImageSource(FridgeItem fridgeItem)
      {
         if (fridgeItem.ItemImage == null || fridgeItem.ItemImage.Length == 0)
         {
            return $"images/french-fries.png";
         }

         return $"data:image;base64,{Convert.ToBase64String(fridgeItem.ItemImage)}";
      }
   }
}
