using Frinfo.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Frinfo.Client.Services
{
   public class HouseholdDataService : IHouseholdDataService
   {
      private readonly HttpClient httpClient;

      public HouseholdDataService(HttpClient httpClient)
      {
         httpClient.BaseAddress = new Uri("https://localhost:44304");
         this.httpClient = httpClient;
      }

      public async Task<Household> GetHouseholdById(int householdId)
      {
         var response = await httpClient.GetAsync($"api/household/{householdId}", HttpCompletionOption.ResponseHeadersRead);
         if (response.IsSuccessStatusCode)
         {
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
               return await JsonSerializer.DeserializeAsync<Household>(stream,  new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
            }
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
               return await JsonSerializer.DeserializeAsync<Household>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
               return await JsonSerializer.DeserializeAsync<Household>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
         }

         return null;
      }
   }
}
