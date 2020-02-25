using Frinfo.Shared;
using System.Collections.Generic;

namespace Frinfo.API.Model
{
   public interface IHouseholdRepository
   {
      Household GetHouseholdById(int householdId);

      Household GetHouseholdByCode(string code);
   }
}
