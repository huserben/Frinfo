using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Frinfo.Shared
{
   public class Fridge
   {
      public int FridgeId { get; set; }

      public int HouseholdId { get; set; }

      public Household Household { get; set; }

      [Required]
      public string Name { get; set; }

      public virtual ICollection<FridgeItem> Items { get; set; }
   }
}
