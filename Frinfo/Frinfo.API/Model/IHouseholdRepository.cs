using Frinfo.Shared;
using System.Threading.Tasks;

namespace Frinfo.API.Model
{
   public interface IHouseholdRepository
   {
      Household GetHouseholdById(int householdId);

      Household GetHouseholdByCode(string code);

      Task<bool> DeleteHouseholdById(int id);

      Task<Household> AddNewHousehold(string name);
   }
}
