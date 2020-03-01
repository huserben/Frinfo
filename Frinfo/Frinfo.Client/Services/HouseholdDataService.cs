using Blazored.LocalStorage;
using Frinfo.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Frinfo.Client.Services
{
   public class HouseholdDataService : IHouseholdDataService
   {
      private const string RecentHouseholdsKey = "RecentHouseholds";

      private readonly IHttpClient httpClient;
      private readonly ILocalStorageService localStorageService;

      public HouseholdDataService(IHttpClient httpClient, ILocalStorageService LocalStorageService)
      {
         this.httpClient = httpClient;
         localStorageService = LocalStorageService;
      }

      public async Task<Household> GetHouseholdById(int householdId)
      {
         var response =
            await httpClient.GetAsync($"api/household/{householdId}", HttpCompletionOption.ResponseHeadersRead);
         if (response.IsSuccessStatusCode)
         {
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
               return await JsonSerializer.DeserializeAsync<Household>(stream,
                  new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
         }

         return null;
      }

      public async Task<Household> GetHouseholdByCode(string householdCode)
      {
         var response = await httpClient.GetAsync($"api/household?code={householdCode}",
            HttpCompletionOption.ResponseHeadersRead);
         if (response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.OK)
         {
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
               var household = await JsonSerializer.DeserializeAsync<Household>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

               await AddHouseholdToLocalStorage(household);

               return household;
            }
         }

         return null;
      }

      public async Task<bool> DeleteHousehold(int householdId)
      {
         var response = await httpClient.DeleteAsync($"api/household/{householdId}");

         return response.IsSuccessStatusCode;
      }

      public async Task<Household> AddNewHousehold(string newHouseholdName)
      {
         var response = await httpClient.PostAsync($"api/household?name={newHouseholdName}", null);

         if (response.IsSuccessStatusCode)
         {
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
               var newHousehold = await JsonSerializer.DeserializeAsync<Household>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
               await AddHouseholdToLocalStorage(newHousehold);

               return newHousehold;
            }
         }

         return null;
      }

      public async Task<IEnumerable<Household>> GetLocallyStoredHouseholds()
      {
         var containsFavoriteHouseholds = await localStorageService.ContainKeyAsync(RecentHouseholdsKey);
         if (!containsFavoriteHouseholds)
         {
            await localStorageService.SetItemAsync(RecentHouseholdsKey, new List<string>());
         }

         var localHouseholds = await LoadHouseholdsFromLocalStorage();
         return localHouseholds.Select(h => JsonSerializer.Deserialize<Household>(h));
      }

      public async Task RemoveHouseholdFromLocalStorage(int householdId)
      {
         var localHouseholds = (await GetLocallyStoredHouseholds()).ToList();
         var householdToRemove = localHouseholds.FirstOrDefault(h => h.HouseholdId == householdId);
         localHouseholds.Remove(householdToRemove);

         await StoreHouseholdsInLocalStorage(localHouseholds.Select(h => JsonSerializer.Serialize(h)).ToList());
      }

      private async Task AddHouseholdToLocalStorage(Household household)
      {
         var localHouseholds = await LoadHouseholdsFromLocalStorage();
         localHouseholds.Insert(0, JsonSerializer.Serialize(household));
         await StoreHouseholdsInLocalStorage(localHouseholds);
      }

      private async Task StoreHouseholdsInLocalStorage(List<string> households)
      {
         await localStorageService.SetItemAsync(RecentHouseholdsKey, households);
      }

      private async Task<List<string>> LoadHouseholdsFromLocalStorage()
      {
         return await localStorageService.GetItemAsync<List<string>>(RecentHouseholdsKey);
      }
   }
}
