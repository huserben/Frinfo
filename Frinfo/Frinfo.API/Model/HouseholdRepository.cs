using Frinfo.Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Frinfo.API.Model
{
   public class HouseholdRepository : IHouseholdRepository
   {
      private readonly AppDbContext dbContext;

      public HouseholdRepository(AppDbContext dbContext)
      {
         this.dbContext = dbContext;
      }

      public Household GetHouseholdByCode(string code)
      {
         return dbContext.Households.Include(h => h.Fridges).FirstOrDefault(x => x.HouseholdCode == code);
      }

      public Household GetHouseholdById(int householdId)
      {
         return dbContext.Households.Include(h => h.Fridges).Single(x => x.HouseholdId == householdId);
      }
   }
}
