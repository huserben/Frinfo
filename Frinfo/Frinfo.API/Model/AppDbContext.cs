using Frinfo.Shared;
using Microsoft.EntityFrameworkCore;
using System;

namespace Frinfo.API.Model
{
   public class AppDbContext : DbContext
   {
      public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
      {

      }

      public DbSet<Household> Households { get; set; }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         base.OnModelCreating(modelBuilder);

         var fridgeItem1 = new FridgeItem { FridgeId = 2, FridgeItemId = 1, Name = "French Fries", ExpirationDate = DateTime.Parse("31.03.2022") };
         var fridgeItem2 = new FridgeItem { FridgeId = 2, FridgeItemId = 2, Name = "Chicken Nuggets" };
         var fridgeItem3 = new FridgeItem { FridgeId = 1, FridgeItemId = 3, Name = "Salmon", ExpirationDate = DateTime.Parse("03.03.2020") };
         var fridgeItem4 = new FridgeItem { FridgeId = 1, FridgeItemId = 4, Name = "Ice Cream" };

         var fridge1 = new Fridge { HouseholdId = 1, FridgeId = 1, Name = "Kitchen" };
         var fridge2 = new Fridge { HouseholdId = 1, FridgeId = 2, Name = "Cellar" };

         modelBuilder.Entity<Household>().HasData(new Household { HouseholdId = 1, Name = "Test Household", HouseholdCode = "1337" });

         modelBuilder.Entity<FridgeItem>().HasData(fridgeItem1);
         modelBuilder.Entity<FridgeItem>().HasData(fridgeItem2);
         modelBuilder.Entity<FridgeItem>().HasData(fridgeItem3);
         modelBuilder.Entity<FridgeItem>().HasData(fridgeItem4);
         modelBuilder.Entity<Fridge>().HasData(fridge1);
         modelBuilder.Entity<Fridge>().HasData(fridge2);
      }
   }
}
