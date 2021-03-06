﻿using Blazored.LocalStorage;
using Frinfo.Shared;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Frinfo.Client.Services
{
   public class LocalStorageHouseholdService : ILocalStorageHouseholdService
   {
      private const string RecentHouseholdsKey = "RecentHouseholds";

      private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

      private readonly ILocalStorageService localStorageService;
      private readonly ILogger<LocalStorageHouseholdService> logger;
      private Dictionary<int, Household> storedHouseholds;
      private bool hasChanges = false;

      public LocalStorageHouseholdService(ILocalStorageService localStorageService, ILogger<LocalStorageHouseholdService> logger)
      {
         this.localStorageService = localStorageService;
         this.logger = logger;

         var storageTimer = new System.Timers.Timer(10000);
         storageTimer.AutoReset = true;
         storageTimer.Elapsed += OnStorageTimerHasElapsed;
         storageTimer.Enabled = true;
      }

      public async Task<bool> AddOrUpdateFridge(Fridge fridge)
      {
         var (fridgeToRemove, household) = await GetFridgeById(fridge.HouseholdId, fridge.FridgeId);

         if (fridgeToRemove != null)
         {
            household.Fridges.Remove(fridgeToRemove);
         }

         household.Fridges.Add(fridge);
         hasChanges = true;

         return true;
      }

      public async Task AddOrUpdateFridgeItem(int householdId, FridgeItem addedFridgeItem)
      {
         var (fridgeItemToRemove, fridge) = await GetFridgeItemById(householdId, addedFridgeItem.FridgeId, addedFridgeItem.FridgeItemId);

         if (fridgeItemToRemove != null)
         {
            fridge.Items.Remove(fridgeItemToRemove);
         }

         fridge.Items.Add(addedFridgeItem);

         hasChanges = true;
      }

      public async Task AddOrUpdateHousehold(Household household)
      {
         var localHouseholds = await GetStoredHouseholds();
         localHouseholds[household.HouseholdId] = household;

         hasChanges = true;
      }

      public async Task<Fridge> GetLocallyStoredFridge(int householdId, int fridgeId)
      {
         var (fridge, _) = await GetFridgeById(householdId, fridgeId);
         return fridge;
      }

      public async Task<IEnumerable<Household>> GetLocallyStoredHouseholds()
      {
         var householdsDictionary = await GetStoredHouseholds();
         return householdsDictionary.Values;
      }

      public async Task<IEnumerable<FridgeItem>> GetLocallyStoredFridgeIems(int? householdId = null, int? fridgeId = null)
      {
         logger.LogDebug("Getting all locally stored fridge items");

         var fridgeItems = new List<FridgeItem>();

         var householdsDictionary = await GetStoredHouseholds();

         IEnumerable<Household> households = householdsDictionary.Values;

         if (householdId.HasValue)
         {
            logger.LogDebug($"Household Parameter specified: {householdId.Value}");
            households = households.Where(h => h.HouseholdId == householdId.Value);
         }

         foreach (var household in households)
         {
            IEnumerable<Fridge> fridges = household.Fridges;

            if (fridgeId.HasValue)
            {
               logger.LogDebug($"Fridge Parameter specified: {fridgeId.Value}");
               fridges = fridges.Where(x => x.FridgeId == fridgeId.Value);
            }

            foreach (var fridge in fridges)
            {
               fridgeItems.AddRange(fridge.Items);
            }
         }

         return fridgeItems;
      }

      public async Task<bool> RemoveFridge(int householdId, int fridgeId)
      {
         var localHouseholds = await GetStoredHouseholds();

         if (localHouseholds.ContainsKey(householdId))
         {
            var household = localHouseholds[householdId];
            var fridgeToRemove = household.Fridges.FirstOrDefault(f => f.FridgeId == fridgeId);

            if (household.Fridges.Remove(fridgeToRemove))
            {
               hasChanges = true;
               return true;
            }
         }

         return false;
      }

      public async Task<bool> RemoveFridgeItem(int householdId, int fridgeId, int fridgeItemId)
      {
         var (fridgeItemToRemove, fridge) = await GetFridgeItemById(householdId, fridgeId, fridgeItemId);

         if (fridgeItemToRemove != null && fridge.Items.Remove(fridgeItemToRemove))
         {
            hasChanges = true;
            return true;
         }

         return false;
      }

      public async Task<bool> RemoveHousehold(int householdId)
      {
         var localHouseholds = await GetStoredHouseholds();

         if (localHouseholds.ContainsKey(householdId))
         {
            hasChanges = true;
            return localHouseholds.Remove(householdId);
         }

         return false;
      }

      private async Task<(Fridge fridge, Household household)> GetFridgeById(int householdId, int fridgeId)
      {
         var storedHouseholds = await GetStoredHouseholds();
         var household = storedHouseholds[householdId];
         var fridge = household.Fridges.FirstOrDefault(f => f.FridgeId == fridgeId);
         return (fridge, household);
      }

      private async Task<(FridgeItem fridgeItem, Fridge fridge)> GetFridgeItemById(int householdId, int fridgeId, int fridgeItemId)
      {
         var (fridge, _) = await GetFridgeById(householdId, fridgeId);
         var fridgeItem = fridge.Items.FirstOrDefault(i => i.FridgeItemId == fridgeItemId);

         return (fridgeItem, fridge);
      }

      private async Task StoreHouseholdsInLocalStorage()
      {
         if (!hasChanges)
         {
            logger.LogDebug("No changes to store");
            return;
         }

         var storedHouseholds = await GetStoredHouseholds();

         logger.LogDebug("Storing Dictionary...");
         await localStorageService.SetItemAsync(RecentHouseholdsKey, storedHouseholds.Values.Select(h => JsonSerializer.Serialize(h)).ToList());

         hasChanges = false;
         logger.LogDebug("Stored Successfully");
      }

      private async Task<Dictionary<int, Household>> GetStoredHouseholds()
      {
         await semaphoreSlim.WaitAsync();

         try
         {
            if (storedHouseholds == null)
            {
               logger.LogDebug("Loading data from local storage");

               storedHouseholds = new Dictionary<int, Household>();

               try
               {
                  var containsFavoriteHouseholds = await localStorageService.ContainKeyAsync(RecentHouseholdsKey);
                  if (containsFavoriteHouseholds)
                  {
                     var householdsAsJson = await localStorageService.GetItemAsync<List<string>>(RecentHouseholdsKey);

                     foreach (var household in householdsAsJson.Select(h => JsonSerializer.Deserialize<Household>(h)))
                     {
                        logger.LogDebug($"Loaded household {household.HouseholdId}");

                        storedHouseholds.Add(household.HouseholdId, household);
                     }
                  }
               }
               catch
               {
                  logger.LogDebug("Error during loading of local storage - initializing empty.");
               }
            }
         }
         finally
         {
            semaphoreSlim.Release();
         }

         return storedHouseholds;
      }


      private async void OnStorageTimerHasElapsed(object sender, ElapsedEventArgs e)
      {
         logger.LogDebug("Storage Timer Elapsed");
         await StoreHouseholdsInLocalStorage();
      }
   }
}
