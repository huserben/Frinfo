using Frinfo.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frinfo.Client.Services
{
   public interface IHouseholdDataService
   {
      Task<IEnumerable<Household>> GetAllHouseholds();

      Task<Household> GetHouseholdById(int householdId);
   }
}
