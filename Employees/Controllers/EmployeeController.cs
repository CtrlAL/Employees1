using Employees.Models;
using Microsoft.AspNetCore.Mvc;
using DAL.Interfaces;
using QueryParams;
using Employees.Models.CreateModels;

namespace Employees.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmlployeeRepository _eployeeRepository;

        public EmployeeController(IEmlployeeRepository eployeeRepository)
        {
            _eployeeRepository = eployeeRepository;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateEmployeeModel model)
        {
            var id = await _eployeeRepository.CreateAsync(CreateEmployeeModel.ToEntity(model));

            return Ok(new { id });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> Delete([FromRoute] int id)
        {
            var result = await _eployeeRepository.DeleteAsync(id);

            return Ok(new { deleted = result });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<int>> Update([FromRoute] int id, [FromBody] EmployeeModel model)
        {
            model.Id = id;
            var result = await _eployeeRepository.UpdateAsync(EmployeeModel.ToEntity(model));

            return Ok(new { updated = result });
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<EmployeeModel>> Get([FromRoute] int id)
        {
            var result = await _eployeeRepository.GetAsync(id);

            return  result != null ? Ok(EmployeeModel.ToModel(result)) : NotFound();
        }

        [HttpGet]
        [Route("filter")]
        public async Task<ActionResult<IList<EmployeeModel>>> GetList([FromQuery] EmployeesQueryParams queryParams)
        {
            var result = await _eployeeRepository.GetAsync(queryParams);

            return Ok(result.Select(EmployeeModel.ToModel).ToList());
        }
    }
}
