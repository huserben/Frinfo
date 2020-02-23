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
      public IActionResult GetHouseholds()
      {
         return Ok(householdRepistory.GetAllHouseholds());
      }

      [HttpGet("{id}")]
      public IActionResult GetHouseholdById(int id)
      {
         return Ok(householdRepistory.GetHouseholdById(id));
      }
   }
}