using System.Threading.Tasks;
using Frinfo.API.Model;
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

      [HttpPost("{householdId}/fridge")]
      public async Task<IActionResult> AddNewFridgeAsync(int householdId, string name)
      {
         var newFridge = await householdRepistory.AddFridge(householdId, name);

         if (newFridge != null)
         {
            return Created($"api/household/fridge/{newFridge.FridgeId}", newFridge);
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
   }
}