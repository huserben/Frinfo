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
         return dbContext.Households.Include(h => h.Fridges).FirstOrDefault(x => x.HouseholdCode == code);
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

         var newHousehold = new Household {HouseholdCode = code, Name = name};
         dbContext.Households.Add(newHousehold);

         await dbContext.SaveChangesAsync();

         return newHousehold;
      }

      public Household GetHouseholdById(int householdId)
      {
         return dbContext.Households.Include(h => h.Fridges).Single(x => x.HouseholdId == householdId);
      }

      private static string GenerateRandomString(int length)
      {
         return new string(Enumerable.Repeat(HouseholdCodeCharacters, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
      }
   }
}
