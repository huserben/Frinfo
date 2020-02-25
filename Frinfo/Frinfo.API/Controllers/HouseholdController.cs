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
         return Ok(householdRepistory.GetHouseholdById(id));
      }
   }
}