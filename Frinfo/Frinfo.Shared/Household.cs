using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Frinfo.Shared
{
   public class Household
   {
      public int HouseholdId { get; set; }

      [Required]
      public string Name { get; set; }

      [Required]
      public string HouseholdCode { get; set; }

      public ICollection<Fridge> Fridges { get; set; }
   }
}
