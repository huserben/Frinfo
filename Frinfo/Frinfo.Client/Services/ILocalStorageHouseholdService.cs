using Frinfo.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frinfo.Client.Services
{
   public interface ILocalStorageHouseholdService
   {
      Task<IEnumerable<Household>> GetLocallyStoredHouseholds();
      
      Task<IEnumerable<FridgeItem>> GetLocallyStoredFridgeIems(int? householdId = null, int? fridgeId = null);

      Task AddOrUpdateHousehold(Household household);

      Task<bool> RemoveHousehold(int householdId);

      Task<Fridge> GetLocallyStoredFridge(int householdId, int fridgeId);

      Task<bool> AddOrUpdateFridge(Fridge fridge);

      Task<bool> RemoveFridge(int householdId, int fridgeId);
      
      Task<bool> RemoveFridgeItem(int householdId, int fridgeId, int fridgeItemId);

      Task AddOrUpdateFridgeItem(int householdId, FridgeItem addedFridgeItem);
   }
}
