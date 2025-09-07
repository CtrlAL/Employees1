using Employees.Models;
using Microsoft.AspNetCore.Mvc;

namespace Employees.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] EmployeeModel model)
        {

            return Ok(new { id = 1 });
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<int>> Delete([FromRoute] int id)
        {

            return Ok(new { deleted = true });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<int>> Update([FromRoute] int id, [FromBody] EmployeeModel model)
        {

            return Ok(new { deleted = true });
        }

        [HttpGet]
        [Route("filter")]
        public async Task<ActionResult<int>> Get([FromRoute] int id, [FromBody] EmployeeModel model)
        {

            return Ok(new { deleted = true });
        }
    }
}
