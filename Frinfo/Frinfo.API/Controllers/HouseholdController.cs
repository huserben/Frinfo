using System.Threading.Tasks;
using Frinfo.API.Model;
using Frinfo.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Frinfo.API.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class HouseholdController : ControllerBase
   {
      private readonly IHouseholdRepository householdRepistory;

      public HouseholdController(IHouseholdRepository householdRepistory)
      {
         this.householdRepistory = householdRepistory;
      }

      [HttpGet]
      public IActionResult GetHouseholdByCode(string code)
      {
         if (string.IsNullOrEmpty(code))
         {
            return BadRequest("Code not supplied");
         }

         var household = householdRepistory.GetHouseholdByCode(code);

         if (household != null)
         {
            return Ok(household);
         }

         return NoContent();
      }

      [HttpGet("{id}")]
      public IActionResult GetHouseholdById(int id)
      {
         var household = householdRepistory.GetHouseholdById(id);

         if (household == null)
         {
            return NotFound(id);
         }

         return Ok(household);
      }

      [HttpGet("{householdId}/fridge/{fridgeId}")]
      public IActionResult GetFridgeById(int householdId, int fridgeId)
      {
         var fridge = householdRepistory.GetFridgeById(householdId, fridgeId);

         if (fridge == null)
         {
            return NotFound(fridgeId);
         }

         return Ok(fridge);
      }

      [HttpDelete("{id}")]
      public async Task<IActionResult> DeleteHouseholdById(int id)
      {
         if (await householdRepistory.DeleteHouseholdById(id))
         {
            return Ok();
         }

         return BadRequest();
      }

      [HttpPost]
      public async Task<IActionResult> AddNewHousehold(string name)
      {
         var newHousehold = await householdRepistory.AddNewHousehold(name);

         return Created($"api/household/{newHousehold.HouseholdId}", newHousehold);
      }

      [HttpPut]
      public async Task<IActionResult> UpdateHousehold([FromBody]Household household)
      {
         var updatedHousehold = await householdRepistory.UpdateHousehold(household);

         if (updatedHousehold != null)
         {
            return NoContent();
         }

         return BadRequest();
      }

      [HttpPost("{householdId}/fridge")]
      public async Task<IActionResult> AddNewFridgeAsync(int householdId, [FromBody]Fridge newFridge)
      {
         var addedFridge = await householdRepistory.AddFridge(householdId, newFridge);

         if (addedFridge != null)
         {
            return Created($"api/household/fridge/{newFridge.FridgeId}", newFridge);
         }

         return BadRequest();
      }

      [HttpPut("{householdId}/fridge")]
      public async Task<IActionResult> UpdateFridgeAsync(int householdId, [FromBody]Fridge fridgeToUpdate)
      {
         var fridge = await householdRepistory.UpdateFridge(householdId, fridgeToUpdate);

         if (fridge != null)
         {
            return NoContent();
         }

         return BadRequest();
      }

      [HttpPost("{householdId}/fridge/{fridgeId}/item")]
      public async Task<IActionResult> AddNewFridgeItemAsync(int householdId, int fridgeId, [FromBody]FridgeItem newFridgeItem)
      {
         var fridge = householdRepistory.GetFridgeById(householdId, fridgeId);
         var fridgeItem = await householdRepistory.AddFridgeItem(fridge, newFridgeItem);

         if (fridgeItem != null)
         {
            return Created($"api/household/fridge/{fridgeItem.FridgeId}", fridgeItem);
         }

         return BadRequest();
      }

      [HttpPut("{householdId}/fridge/{fridgeId}/item")]
      public async Task<IActionResult> UppdateFridgeItem(int householdId, int fridgeId, [FromBody]FridgeItem fridgeItemToUpdate)
      {
         var fridge = householdRepistory.GetFridgeById(householdId, fridgeId);
         var fridgeItem = await householdRepistory.UpdateFridgeItem(fridge, fridgeItemToUpdate);

         if (fridgeItem != null)
         {
            return NoContent();
         }

         return BadRequest();
      }

      [HttpDelete("{householdId}/fridge/{fridgeId}")]
      public async Task<IActionResult> DeleteHouseholdById(int householdId, int fridgeId)
      {
         if (await householdRepistory.DeleteFridgeById(householdId, fridgeId))
         {
            return Ok();
         }

         return BadRequest();
      }

      [HttpDelete("{householdId}/fridge/{fridgeId}/item/{fridgeItemId}")]
      public async Task<IActionResult> DeleteHouseholdById(int householdId, int fridgeId, int fridgeItemId)
      {
         if (await householdRepistory.DeleteFridgeItemById(householdId, fridgeId, fridgeItemId))
         {
            return Ok();
         }

         return BadRequest();
      }
   }
}