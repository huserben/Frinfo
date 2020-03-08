using System;
using Frinfo.Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frinfo.API.Model
{
   public class HouseholdRepository : IHouseholdRepository
   {
      private const string HouseholdCodeCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

      private static readonly Random random = new Random();

      private readonly AppDbContext dbContext;

      public HouseholdRepository(AppDbContext dbContext)
      {
         this.dbContext = dbContext;
      }

      public Household GetHouseholdByCode(string code)
      {
         return dbContext.Households.Include(h => h.Fridges).ThenInclude(f => f.Items).FirstOrDefault(x => x.HouseholdCode == code);
      }

      public async Task<bool> DeleteHouseholdById(int id)
      {
         var householdToDelete = GetHouseholdById(id);

         if (householdToDelete == null)
         {
            return false;
         }

         dbContext.Households.Remove(householdToDelete);

         await dbContext.SaveChangesAsync();

         return true;
      }

      public async Task<Household> AddNewHousehold(string name)
      {
         var code = string.Empty;
         var codeIsUnique = false;
         while (!codeIsUnique)
         {
            code = GenerateRandomString(4);
            codeIsUnique = GetHouseholdByCode(code) == null;
         }

         var newHousehold = new Household { HouseholdCode = code, Name = name };
         dbContext.Households.Add(newHousehold);

         await dbContext.SaveChangesAsync();

         return newHousehold;
      }

      public async Task<bool> DeleteFridgeById(int householdId, int fridgeId)
      {
         var household = GetHouseholdById(householdId);
         if (household != null)
         {
            var fridgeToRemove = household.Fridges.FirstOrDefault(f => f.FridgeId == fridgeId);
            if (fridgeToRemove != null)
            {
               household.Fridges.Remove(fridgeToRemove);

               await dbContext.SaveChangesAsync();

               return true;
            }
         }

         return false;
      }

      public async Task<Fridge> AddFridge(int householdId, string name)
      {
         var household = GetHouseholdById(householdId);

         if (household != null)
         {
            var fridge = new Fridge { Name = name, Items = new List<FridgeItem>() };
            household.Fridges.Add(fridge);

            await dbContext.SaveChangesAsync();

            return fridge;
         }

         return null;
      }

      public Fridge GetFridgeById(int householdId, int fridgeId)
      {
         var household = GetHouseholdById(householdId);

         if (household != null)
         {
            var fridge = household.Fridges.FirstOrDefault(f => f.FridgeId == fridgeId);
            return fridge;
         }

         return null;
      }

      public async Task<bool> DeleteFridgeItemById(int householdId, int fridgeId, int fridgeItemId)
      {
         var household = GetHouseholdById(householdId);
         if (household != null)
         {
            var fridge = household.Fridges.FirstOrDefault(f => f.FridgeId == fridgeId);
            if (fridge != null)
            {
               var itemToRemove = fridge.Items.FirstOrDefault(i => i.FridgeItemId == fridgeItemId);

               if (itemToRemove != null)
               {
                  fridge.Items.Remove(itemToRemove);
                  await dbContext.SaveChangesAsync();

                  return true;
               }
            }
         }

         return false;
      }

      public async Task<FridgeItem> AddFridgeItem(Fridge fridge, FridgeItem newFridgeItem)
      {
         fridge.Items.Add(newFridgeItem);
         await dbContext.SaveChangesAsync();

         return newFridgeItem;
      }

      public async Task<FridgeItem> UpdateFridgeItem(Fridge fridge, FridgeItem fridgeItemToUpdate)
      {
         var fridgeItem = fridge.Items.FirstOrDefault(fi => fi.FridgeItemId == fridgeItemToUpdate.FridgeItemId);
         if (fridgeItem != null)
         {
            fridgeItem.Name = fridgeItemToUpdate.Name;
            fridgeItem.ExpirationDate = fridgeItemToUpdate.ExpirationDate;
            fridgeItem.ItemImage = fridgeItemToUpdate.ItemImage;
            
            await dbContext.SaveChangesAsync();
         }

         return fridgeItem;
      }

      public Household GetHouseholdById(int householdId)
      {
         return dbContext.Households.Include(h => h.Fridges).ThenInclude(f => f.Items).FirstOrDefault(x => x.HouseholdId == householdId);
      }

      private static string GenerateRandomString(int length)
      {
         return new string(Enumerable.Repeat(HouseholdCodeCharacters, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
      }
   }
}
