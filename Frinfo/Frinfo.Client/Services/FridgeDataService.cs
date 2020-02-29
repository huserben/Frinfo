using System;
using System.Net.Http;
using Frinfo.Shared;
using System.Threading.Tasks;
using System.Text.Json;

namespace Frinfo.Client.Services
{
   public class FridgeDataService : IFridgeDataService
   {
      private HttpClient httpClient;

      public FridgeDataService(HttpClient httpClient)
      {
         httpClient.BaseAddress = new Uri("https://localhost:44304");
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
   }
}
