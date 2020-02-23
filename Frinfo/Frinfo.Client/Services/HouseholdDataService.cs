using Frinfo.Shared;
using System;
using System.Collections.Generic;
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

      public async Task<IEnumerable<Household>> GetAllHouseholds()
      {
         var housholdData = await httpClient.GetStreamAsync($"api/household");
         return await JsonSerializer.DeserializeAsync<IEnumerable<Household>>(housholdData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
      }

      public async Task<Household> GetHouseholdById(int householdId)
      {
         var household = await httpClient.GetStreamAsync($"api/household/{householdId}");
         return await JsonSerializer.DeserializeAsync<Household>(household, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
      }
   }
}
