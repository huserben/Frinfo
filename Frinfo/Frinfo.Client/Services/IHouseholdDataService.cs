using Frinfo.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frinfo.Client.Services
{
   public interface IHouseholdDataService
   {
      Task<Household> GetHouseholdById(int householdId);

      Task<Household> GetHouseholdByCode(string householdCode);
   }
}
