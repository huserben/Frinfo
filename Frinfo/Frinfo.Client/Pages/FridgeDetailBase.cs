﻿using Frinfo.Client.Services;
using Frinfo.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Blazor.FileReader;
using System;
using Caliburn.Micro;
using Frinfo.Client.Events;
using System.Threading;
using Frinfo.Client.Components;
using Blazored.Toast.Services;

namespace Frinfo.Client.Pages
{
   public class FridgeDetailBase : ComponentBase, IHandle<OnlineStateChangedEvent>
   {

      [Inject]
      public IFridgeDataService FridgeDataService { get; set; }

      [Inject]
      public IFileReaderService FileReaderService { get; set; }

      [Inject]
      public IHttpClient FrinfoHttpClient { get; set; }

      [Inject]
      public IEventAggregator EventAggregator { get; set; }

      [Inject]
      public IToastService ToastService { get; set; }

      [Inject]
      public NavigationManager NavigationManager { get; set; }

      [Parameter]
      public string HouseholdId { get; set; }

      [Parameter]
      public string FridgeId { get; set; }

      public bool IsOffline { get; private set; }

      public Fridge Fridge { get; private set; } = new Fridge();

      public List<FridgeItem> FridgeItems { get; } = new List<FridgeItem>();

      protected FridgeItemEditComponent EditFridgeItem { get; set; }

      protected FridgeEditComponent EditFridge { get; set; }

      public Task HandleAsync(OnlineStateChangedEvent message, CancellationToken cancellationToken)
      {
         IsOffline = !message.IsOnline;
         return Task.CompletedTask;
      }

      protected override async Task OnInitializedAsync()
      {
         await ReloadFridgeItems();

         IsOffline = !FrinfoHttpClient.IsOnline;

         EventAggregator.SubscribeOnPublishedThread(this);
      }

      protected void EditItem(FridgeItem fridgeItem)
      {
         EditFridgeItem.FridgeItem = fridgeItem;
         EditFridgeItem.Show();
      }

      protected async Task DeleteFridgeItem(FridgeItem fridgeItem)
      {
         var wasRemoveSuccessful = await FridgeDataService.DeleteFridgeItem(Fridge.HouseholdId, Fridge.FridgeId, fridgeItem.FridgeItemId);

         if (wasRemoveSuccessful)
         {
            FridgeItems.Remove(fridgeItem);
            StateHasChanged();
            ToastService.ShowInfo($"Successfully removed {fridgeItem.Name}");
         }
         else
         {
            ToastService.ShowError($"Could not remove {fridgeItem.Name}", "Delete Failed");
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

      protected async void EditFridgeItem_OnClose()
      {
         await ReloadFridgeItems();

         StateHasChanged();
      }

      protected void OnEditFridgeTitle()
      {
         EditFridge.Fridge = Fridge;
         EditFridge.Show();
      }

      protected async void OnRemoveFridge()
      {
         var wasRemoveSuccessful = await FridgeDataService.DeleteFridge(Fridge.HouseholdId, Fridge.FridgeId);

         if (wasRemoveSuccessful)
         { 
            NavigationManager.NavigateTo($"household/{Fridge.HouseholdId}");
            ToastService.ShowInfo($"Removed {Fridge.Name}");
         }
         else
         {
            ToastService.ShowError($"Failed to remove {Fridge.Name}", "Delete Failed");
         }
      }

      protected async void EditFridge_OnClose()
      {
         Fridge = await FridgeDataService.GetFridgeById(int.Parse(HouseholdId), int.Parse(FridgeId));
         StateHasChanged();
      }

      protected void AddFridgeItem()
      {
         EditFridgeItem.FridgeItem = new FridgeItem();
         EditFridgeItem.Show();
      }

      protected void BackToHousehold()
      {
         NavigationManager.NavigateTo($"household/{Fridge.HouseholdId}");
      }

      private async Task ReloadFridgeItems()
      {
         FridgeItems.Clear();
         Fridge = await FridgeDataService.GetFridgeById(int.Parse(HouseholdId), int.Parse(FridgeId));
         FridgeItems.AddRange(Fridge.Items);
      }
   }
}
