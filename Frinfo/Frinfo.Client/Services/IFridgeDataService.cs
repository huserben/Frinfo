using System.Threading.Tasks;
using Frinfo.Shared;

namespace Frinfo.Client.Services
{
   public interface IFridgeDataService
   {
      Task<Fridge> AddNewFridge(Fridge fridge);

      Task<bool> UpdateFridge(Fridge fridge);

      Task<bool> DeleteFridge(int householdId, int fridgeId);

      Task<Fridge> GetFridgeById(int householdId, int fridgeId);
      
      Task<bool> DeleteFridgeItem(int householdId, int fridgeId, int fridgeItemId);

      Task<FridgeItem> AddFridgeItem(int householdId, FridgeItem fridgeItem);

      Task<bool> UpdateFridgeItem(int householdId, FridgeItem fridgeItem);
   }
}
