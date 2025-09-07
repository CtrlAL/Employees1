using Entity;

namespace Employees.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public int CompanyId { get; set; }
        public DepartmentModel Department { get; set; }
        public PassportModel Passport { get; set; }

        public static EmployeeModel ToModel(Employee entity)
        {
            return new EmployeeModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Phone = entity.Phone,
                CompanyId = entity.CompanyId,
                Department = DepartmentModel.FromEntity(entity.Department),
                Passport = PassportModel.FromEntity(entity.Passport),
            };
        }

        public static Employee ToEntity(EmployeeModel model)
        {
            return new Employee(model.Id,
                model.CompanyId,
                model.Name,
                model.Surname,
                model.Phone);
        }
    }
}
