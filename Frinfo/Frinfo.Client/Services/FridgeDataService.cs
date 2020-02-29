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

      public FridgeDataService(IHttpClient httpClient)
      {
         this.httpClient = httpClient;
      }

      public async Task<Fridge> AddNewFridge(int householdId,  string fridgeName)
      {
         var response = await httpClient.PostAsync($"api/household/{householdId}/fridge?name={fridgeName}", null);

         if (response.IsSuccessStatusCode)
         {
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
               return await JsonSerializer.DeserializeAsync<Fridge>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
         }

         return null;
      }

      public async Task<bool> DeleteFridge(int householdId, int fridgeId)
      {
         var response = await httpClient.DeleteAsync($"api/household/{householdId}/fridge/{fridgeId}");

         return response.IsSuccessStatusCode;
      }

      public async Task<Fridge> GetFridgeById(int householdId, int fridgeId)
      {
         var response = await httpClient.GetAsync($"api/household/{householdId}/fridge/{fridgeId}", System.Net.Http.HttpCompletionOption.ResponseHeadersRead);

         if (response.IsSuccessStatusCode)
         {
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
               return await JsonSerializer.DeserializeAsync<Fridge>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
         }

         return null;
      }

      public async Task<bool> DeleteFridgeItem(int householdId, int fridgeId, int fridgeItemId)
      {
         var response = await httpClient.DeleteAsync($"api/household/{householdId}/fridge/{fridgeId}/item/{fridgeItemId}");

         return response.IsSuccessStatusCode;
      }

      public async Task<FridgeItem> AddFridgeItem(int householdId, FridgeItem fridgeItem)
      {
         var fridgeJson = new StringContent(JsonSerializer.Serialize(fridgeItem), Encoding.UTF8, "application/json");

         var response = await httpClient.PostAsync($"api/household/{householdId}/fridge/{fridgeItem.FridgeId}/item", fridgeJson);

         if (response.IsSuccessStatusCode)
         {
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
               return await JsonSerializer.DeserializeAsync<FridgeItem>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
         }

         return null;
      }
   }
}
