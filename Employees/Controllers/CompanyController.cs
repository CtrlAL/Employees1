using DAL.Interfaces;
using Employees.Models;
using Microsoft.AspNetCore.Mvc;
using QueryParams;

namespace Employees.Controllers
{
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CompanyModel model)
        {
            var id = await _companyRepository.CreateAsync(CompanyModel.ToEntity(model));

            return Ok(new { id });
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<int>> Delete([FromRoute] int id)
        {
            var result = await _companyRepository.DeleteAsync(id);

            return Ok(new { deleted = result });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<int>> Update([FromRoute] int id, [FromBody] CompanyModel model)
        {
            model.Id = id;
            var result = await _companyRepository.UpdateAsync(CompanyModel.ToEntity(model));

            return Ok(new { updated = result });
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CompanyModel>> Get([FromRoute] int id, [FromQuery] EmployeesQueryParams queryParams)
        {
            var result = await _companyRepository.GetAsync(id);

            return Ok(CompanyModel.ToModel(result));
        }

        [HttpGet]
        [Route("filter")]
        public async Task<ActionResult<IList<CompanyModel>>> GetList([FromQuery] EmployeesQueryParams queryParams)
        {
            var result = await _companyRepository.GetAsync(queryParams);

            return Ok(result.Select(CompanyModel.ToModel).ToList());
        }
    }
}
