using Blazored.LocalStorage;
using Frinfo.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Frinfo.Client.Services
{
   public class LocalStorageHouseholdService : ILocalStorageHouseholdService
   {
      private const string RecentHouseholdsKey = "RecentHouseholds";

      private readonly ILocalStorageService localStorageService;

      public LocalStorageHouseholdService(ILocalStorageService localStorageService)
      {
         this.localStorageService = localStorageService;
      }

      public async Task<bool> AddOrUpdateFridge(Fridge fridge)
      {
         var (fridgeToRemove, household, households) = await GetFridgeById(fridge.HouseholdId, fridge.FridgeId);

         if (fridgeToRemove != null)
         {
            household.Fridges.Remove(fridgeToRemove);
         }

         household.Fridges.Add(fridge);
         await StoreHouseholdsInLocalStorage(households);

         return true;
      }

      public async Task AddOrUpdateFridgeItem(int householdId, FridgeItem addedFridgeItem)
      {
         var (fridgeItemToRemove, fridge, households) = await GetFridgeItemById(householdId, addedFridgeItem.FridgeId, addedFridgeItem.FridgeItemId);

         if (fridgeItemToRemove != null)
         {
            fridge.Items.Remove(fridgeItemToRemove);
         }

         fridge.Items.Add(addedFridgeItem);
         await StoreHouseholdsInLocalStorage(households);
      }

      public async Task AddOrUpdateHousehold(Household household)
      {
         var localHouseholds = await GetLocallyStoredHouseholds();

         var householdToRemove = localHouseholds.FirstOrDefault(h => h.HouseholdId == household.HouseholdId);
         if (householdToRemove != null)
         {
            localHouseholds.Remove(householdToRemove);
         }

         localHouseholds.Insert(0, household);
         await StoreHouseholdsInLocalStorage(localHouseholds);
      }

      public async Task<Fridge> GetLocallyStoredFridge(int householdId, int fridgeId)
      {
         var (fridge, _, _) = await GetFridgeById(householdId, fridgeId);
         return fridge;
      }

      public async Task<List<Household>> GetLocallyStoredHouseholds()
      {
         var containsFavoriteHouseholds = await localStorageService.ContainKeyAsync(RecentHouseholdsKey);
         if (!containsFavoriteHouseholds)
         {
            await localStorageService.SetItemAsync(RecentHouseholdsKey, new List<string>());
         }

         var householdsAsJson = await localStorageService.GetItemAsync<List<string>>(RecentHouseholdsKey);
         return householdsAsJson.Select(h => JsonSerializer.Deserialize<Household>(h)).ToList();
      }

      public async Task<bool> RemoveFridge(int householdId, int fridgeId)
      {
         var (fridgeToRemove, _, households) = await GetFridgeById(householdId, fridgeId);

         if (fridgeToRemove == null)
         {
            return false;
         }

         var household = fridgeToRemove.Household;
         household.Fridges.Remove(fridgeToRemove);
         await StoreHouseholdsInLocalStorage(households);

         return true;
      }

      public async Task<bool> RemoveFridgeItem(int householdId, int fridgeId, int fridgeItemId)
      {
         var (fridgeItemToRemove, fridge, households) = await GetFridgeItemById(householdId, fridgeId, fridgeItemId);

         if (fridgeItemToRemove == null)
         {
            return false;
         }

         fridge.Items.Remove(fridgeItemToRemove);
         await StoreHouseholdsInLocalStorage(households);

         return true;
      }

      public async Task<bool> RemoveHousehold(int householdId)
      {
         var localHouseholds = await GetLocallyStoredHouseholds();
         var householdToRemove = localHouseholds.FirstOrDefault(h => h.HouseholdId == householdId);
         if (householdToRemove != null)
         {
            if (localHouseholds.Remove(householdToRemove))
            {
               await StoreHouseholdsInLocalStorage(localHouseholds);
               return true;
            }
         }

         return false;
      }

      public async Task StoreHouseholdsInLocalStorage(List<Household> households)
      {
         var householdsAsJson = households.Select(h => JsonSerializer.Serialize(h));
         await localStorageService.SetItemAsync(RecentHouseholdsKey, householdsAsJson);
      }

      private async Task<(Fridge fridge, Household household, List<Household> households)> GetFridgeById(int householdId, int fridgeId)
      {
         var storedHouseholds = await GetLocallyStoredHouseholds();
         var household = storedHouseholds.FirstOrDefault(h => h.HouseholdId == householdId);
         if (household == null)
         {
            return (null, null, storedHouseholds);
         }

         var fridge = household.Fridges.FirstOrDefault(f => f.FridgeId == fridgeId);
         return (fridge, household, storedHouseholds);
      }

      private async Task<(FridgeItem fridgeItem, Fridge fridge, List<Household> households)> GetFridgeItemById(int householdId, int fridgeId, int fridgeItemId)
      {
         var (fridge, _, households) = await GetFridgeById(householdId, fridgeId);
         var fridgeItem = fridge.Items.FirstOrDefault(i => i.FridgeItemId == fridgeItemId);

         return (fridgeItem, fridge, households);
      }
   }
}
