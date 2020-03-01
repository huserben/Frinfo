using Frinfo.Shared;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Frinfo.Client.Services
{
   public class HouseholdDataService : IHouseholdDataService
   {
      private readonly IHttpClient httpClient;
      private readonly ILocalStorageHouseholdService localStorageHouseholdService;

      public HouseholdDataService(IHttpClient httpClient, ILocalStorageHouseholdService localStorageHouseholdService)
      {
         this.httpClient = httpClient;
         this.localStorageHouseholdService = localStorageHouseholdService;
      }

      public async Task<Household> GetHouseholdById(int householdId)
      {
         if (httpClient.IsOnline)
         {
            var response = await httpClient.GetAsync($"api/household/{householdId}", HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
               using (var stream = await response.Content.ReadAsStreamAsync())
               {
                  var household = await JsonSerializer.DeserializeAsync<Household>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                  await localStorageHouseholdService.AddOrUpdateHousehold(household);

                  return household;
               }
            }
         }
         else
         {
            var locallyStored = await localStorageHouseholdService.GetLocallyStoredHouseholds();
            return locallyStored.FirstOrDefault(h => h.HouseholdId == householdId);
         }

         return null;
      }

      public async Task<Household> GetHouseholdByCode(string householdCode)
      {
         var response = await httpClient.GetAsync($"api/household?code={householdCode}", HttpCompletionOption.ResponseHeadersRead);
         if (response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.OK)
         {
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
               var household = await JsonSerializer.DeserializeAsync<Household>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

               await localStorageHouseholdService.AddOrUpdateHousehold(household);

               return household;
            }
         }

         return null;
      }

      public async Task<bool> DeleteHousehold(int householdId)
      {
         var response = await httpClient.DeleteAsync($"api/household/{householdId}");

         await localStorageHouseholdService.RemoveHousehold(householdId);

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
               await localStorageHouseholdService.AddOrUpdateHousehold(newHousehold);

               return newHousehold;
            }
         }

         return null;
      }
   }
}
