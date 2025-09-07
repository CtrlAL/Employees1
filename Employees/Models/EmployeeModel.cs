using Entity;

namespace Employees.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Phone { get; set; }
        public DepartmentModel? Department { get; set; }
        public PassportModel? Passport { get; set; }

        public static EmployeeModel ToModel(Employee entity)
        {
            return new EmployeeModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Phone = entity.Phone,
                CompanyId = entity.CompanyId,
                Department = DepartmentModel.ToModel(entity.Department),
                Passport = PassportModel.ToModel(entity.Passport),
            };
        }

        public static Employee ToEntity(EmployeeModel model)
        {
            return new Employee
            {
                Id = model.Id,
                CompanyId = model.CompanyId,
                Name = model.Name,
                Surname = model.Surname,
                Phone = model.Phone,
                Department = model.Department == null ? null : DepartmentModel.ToEntity(model.Department),
                Passport = model.Passport == null ? null : PassportModel.ToEntity(model.Passport),
            };
        }
    }
}
