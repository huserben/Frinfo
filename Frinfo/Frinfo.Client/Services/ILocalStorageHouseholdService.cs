using Frinfo.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frinfo.Client.Services
{
   public interface ILocalStorageHouseholdService
   {
      Task<List<Household>> GetLocallyStoredHouseholds();

      Task AddOrUpdateHousehold(Household household);

      Task<bool> RemoveHousehold(int householdId);

      Task<Fridge> GetLocallyStoredFridge(int householdId, int fridgeId);

      Task<bool> AddOrUpdateFridge(Fridge fridge);

      Task<bool> RemoveFridge(int householdId, int fridgeId);
      
      Task<bool> RemoveFridgeItem(int householdId, int fridgeId, int fridgeItemId);

      Task AddOrUpdateFridgeItem(FridgeItem addedFridgeItem);
   }
}
