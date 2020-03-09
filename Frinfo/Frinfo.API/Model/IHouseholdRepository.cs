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

      Task<bool> DeleteFridgeById(int householdId, int fridgeId);

      Task<Fridge> AddFridge(int hosueholdId, Fridge newFridge);

      Task<Fridge> UpdateFridge(int householdId, Fridge fridgeToUpdate);

      Fridge GetFridgeById(int householdId, int fridgeId);

      Task<bool> DeleteFridgeItemById(int householdId, int fridgeId, int fridgeItemId);
      
      Task<FridgeItem> AddFridgeItem(Fridge fridge, FridgeItem newFridgeItem);

      Task<FridgeItem> UpdateFridgeItem(Fridge fridge, FridgeItem fridgeItemToUpdate);
   }
}
