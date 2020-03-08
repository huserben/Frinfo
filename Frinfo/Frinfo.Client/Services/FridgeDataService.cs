using Frinfo.Shared;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System.Text;

namespace Frinfo.Client.Services
{
   public class FridgeDataService : IFridgeDataService
   {
      private readonly IHttpClient httpClient;
      private readonly ILocalStorageHouseholdService localStorageHouseholdService;

      public FridgeDataService(IHttpClient httpClient, ILocalStorageHouseholdService localStorageHouseholdService)
      {
         this.httpClient = httpClient;
         this.localStorageHouseholdService = localStorageHouseholdService;
      }

      public async Task<Fridge> AddNewFridge(int householdId,  string fridgeName)
      {
         var response = await httpClient.PostAsync($"api/household/{householdId}/fridge?name={fridgeName}", null);

         if (response.IsSuccessStatusCode)
         {
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
               var newFridge = await JsonSerializer.DeserializeAsync<Fridge>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

               await localStorageHouseholdService.AddOrUpdateFridge(newFridge);

               return newFridge;
            }
         }

         return null;
      }

      public async Task<bool> DeleteFridge(int householdId, int fridgeId)
      {
         var response = await httpClient.DeleteAsync($"api/household/{householdId}/fridge/{fridgeId}");

         await localStorageHouseholdService.RemoveFridge(householdId, fridgeId);

         return response.IsSuccessStatusCode;
      }

      public async Task<Fridge> GetFridgeById(int householdId, int fridgeId)
      {
         if (httpClient.IsOnline)
         {
            var response = await httpClient.GetAsync($"api/household/{householdId}/fridge/{fridgeId}", System.Net.Http.HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
               using (var stream = await response.Content.ReadAsStreamAsync())
               {
                  var fridge = await JsonSerializer.DeserializeAsync<Fridge>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                  await localStorageHouseholdService.AddOrUpdateFridge(fridge);

                  return fridge;
               }
            }
         }
         else
         {
            var fridge = await localStorageHouseholdService.GetLocallyStoredFridge(householdId, fridgeId);
            return fridge;
         }

         return null;
      }

      public async Task<bool> DeleteFridgeItem(int householdId, int fridgeId, int fridgeItemId)
      {
         var response = await httpClient.DeleteAsync($"api/household/{householdId}/fridge/{fridgeId}/item/{fridgeItemId}");
         await localStorageHouseholdService.RemoveFridgeItem(householdId, fridgeId, fridgeItemId);

         return response.IsSuccessStatusCode;
      }

      public async Task<FridgeItem> AddFridgeItem(int householdId, FridgeItem fridgeItem)
      {
         var fridgeItemJson = new StringContent(JsonSerializer.Serialize(fridgeItem), Encoding.UTF8, "application/json");

         var response = await httpClient.PostAsync($"api/household/{householdId}/fridge/{fridgeItem.FridgeId}/item", fridgeItemJson);

         if (response.IsSuccessStatusCode)
         {
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
               var addedFridgeItem = await JsonSerializer.DeserializeAsync<FridgeItem>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
               await localStorageHouseholdService.AddOrUpdateFridgeItem(householdId, addedFridgeItem);

               return addedFridgeItem;
            }
         }

         return null;
      }

      public async Task<bool> UpdateFridgeItem(int householdId, FridgeItem fridgeItem)
      {
         var fridgeItemJson = new StringContent(JsonSerializer.Serialize(fridgeItem), Encoding.UTF8, "application/json");

         var response = await httpClient.PutAsync($"api/household/{householdId}/fridge/{fridgeItem.FridgeId}/item", fridgeItemJson);

         return response.IsSuccessStatusCode;
      }
   }
}
