using Frinfo.Shared;
using System.Collections.Generic;

namespace Frinfo.API.Model
{
   public interface IHouseholdRepository
   {
      IEnumerable<Household> GetAllHouseholds();

      Household GetHouseholdById(int householdId);
   }
}
