using DAL.Interfaces;
using Employees.Models;
using Microsoft.AspNetCore.Mvc;
using QueryParams;

namespace Employees.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentController(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] DepartmentModel model)
        {
            var id = await _departmentRepository.CreateAsync(DepartmentModel.ToEntity(model));

            return Ok(new { id });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> Delete([FromRoute] int id)
        {
            var result = await _departmentRepository.DeleteAsync(id);

            return Ok(new { deleted = result });
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<IList<DepartmentModel>>> GetList()
        {
            var result = await _departmentRepository.GetAsync(null);

            return Ok(result.Select(DepartmentModel.ToModel).ToList());
        }
    }
}
