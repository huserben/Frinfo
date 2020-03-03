using Blazor.FileReader;
using Caliburn.Micro;
using Frinfo.Client.Events;
using Frinfo.Client.Services;
using Frinfo.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frinfo.Client.Components
{
   public class FridgeItemEditComponentBase : ComponentBase
   {
      protected ElementReference inputTypeFileElement;

      private FridgeItem originalFridgeItem;

      public bool ShowDialog { get; set; }

      public FridgeItem FridgeItem { get; set; }

      [Parameter]
      public EventCallback<bool> CloseEventCallback { get; set; }

      [Parameter]
      public Fridge Fridge { get; set; }

      [Inject]
      public IFridgeDataService FridgeDataService { get; set; }

      [Inject]
      public IFileReaderService FileReaderService { get; set; }

      public void Show()
      {
         ResetDialog();
         ShowDialog = true;
         StateHasChanged();
      }

      protected override void OnInitialized()
      {
         if (FridgeItem == null)
         {
            FridgeItem = new FridgeItem();
         }

         originalFridgeItem = FridgeItem;
      }

      private void ResetDialog()
      {
         FridgeItem = originalFridgeItem;
      }

      public void Close()
      {
         ShowDialog = false;
         originalFridgeItem = null;
         StateHasChanged();
      }

      protected async Task AddFridgeItem()
      {
         FridgeItem.FridgeId = Fridge.FridgeId;
         FridgeItem.ItemImage = await ReadFile();

         if (FridgeItem.FridgeItemId == 0)
         {
            // New Item
            await FridgeDataService.AddFridgeItem(Fridge.HouseholdId, FridgeItem);
         }
         else
         {
            // Update Existing Item
         }

         ShowDialog = false;
         await CloseEventCallback.InvokeAsync(true);
         StateHasChanged();
      }



      private async Task<byte[]> ReadFile()
      {
         foreach (var file in await FileReaderService.CreateReference(inputTypeFileElement).EnumerateFilesAsync())
         {
            // Read into memory and act
            using (var memoryStream = await file.CreateMemoryStreamAsync(4096))
            {
               return memoryStream.ToArray();
            }
         }

         return new byte[0];
      }
   }
}
