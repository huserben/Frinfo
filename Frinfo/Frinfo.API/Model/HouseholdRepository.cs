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

      public IEnumerable<Household> GetAllHouseholds()
      {
         return dbContext.Households;
      }

      public Household GetHouseholdById(int householdId)
      {
         return dbContext.Households.Include(h => h.Fridges).Single(x => x.HouseholdId == householdId);
      }
   }
}
