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

      public string ImageSource
      {
         get
         {
            return $"data:image;base64,{Convert.ToBase64String(FridgeItem.ItemImage)}";
         }
      }

      public string Title
      {
         get
         {
            if (FridgeItem.FridgeItemId == 0)
            {
               return "Add new Fridge Item";
            }

            return $"Edit {FridgeItem.Name}";
         }
      }

      public void Show()
      {
         ShowDialog = true;
         StateHasChanged();
      }

      protected override void OnInitialized()
      {
         if (FridgeItem == null)
         {
            FridgeItem = new FridgeItem();
         }
      }

      public void Close()
      {
         ShowDialog = false;
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
            await FridgeDataService.UpdateFridgeItem(Fridge.HouseholdId, FridgeItem);
         }

         ShowDialog = false;
         await CloseEventCallback.InvokeAsync(true);
         StateHasChanged();
      }

      protected void OnCancelEditItem()
      {
         ShowDialog = false;
         FridgeItem = new FridgeItem();
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
