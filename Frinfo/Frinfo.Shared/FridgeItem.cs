using System;
using System.ComponentModel.DataAnnotations;

namespace Frinfo.Shared
{
   public class FridgeItem
   {
      public int FridgeItemId { get; set; }

      public int FridgeId { get; set; }

      public Fridge Fridge { get; set; }

      [Required]
      public string Name { get; set; }

      public DateTime? ExpirationDate { get; set; }

      public byte[] ItemImage { get; set; }

      public string ItemImageBase64
      {
         get
         {
            if (ItemImage == null)
            {
               return string.Empty;
            }

            return $"data:image;base64,{Convert.ToBase64String(ItemImage)}";
         }
      }
   }
}
