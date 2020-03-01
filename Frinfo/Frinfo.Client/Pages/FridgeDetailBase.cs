﻿using Frinfo.Client.Services;
using Frinfo.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Blazor.FileReader;
using System;

namespace Frinfo.Client.Pages
{
   public class FridgeDetailBase : ComponentBase
   {
      protected ElementReference inputTypeFileElement;

      [Inject]
      public IFridgeDataService FridgeDataService { get; set; }

      [Inject]
      public IFileReaderService FileReaderService { get; set; }

      [Parameter]
      public string HouseholdId { get; set; }

      [Parameter]
      public string FridgeId { get; set; }

      public Fridge Fridge { get; private set; } = new Fridge();

      public List<FridgeItem> FridgeItems { get; } = new List<FridgeItem>();

      public FridgeItem FridgeItem { get; set;  } = new FridgeItem();

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

      protected async Task AddFridgeItem()
      {
         FridgeItem.FridgeId = Fridge.FridgeId;
         FridgeItem.ItemImage = await ReadFile();

         if (FridgeItem.FridgeItemId == 0)
         {
            var addedItem = await FridgeDataService.AddFridgeItem(Fridge.HouseholdId, FridgeItem);

            if (addedItem != null)
            {
               FridgeItems.Add(addedItem);
            }
         }
      }

      protected string GetImageSource(FridgeItem fridgeItem)
      {
         if (fridgeItem.ItemImage == null || fridgeItem.ItemImage.Length == 0)
         {
            return $"images/french-fries.png";
         }

         return $"data:image;base64,{Convert.ToBase64String(fridgeItem.ItemImage)}";
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