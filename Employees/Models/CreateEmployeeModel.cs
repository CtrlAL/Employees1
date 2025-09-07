using Entity;

namespace Employees.Models
{
    public class CreateEmployeeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public PassportModel Passport { get; set; }

        public static Employee ToEntity(CreateEmployeeModel model)
        {
            return new Employee
            {
                Id = model.Id,
                Name = model.Name,
                Surname = model.Surname,
                Phone = model.Phone,
                CompanyId = model.CompanyId,
                DeparmentId = model.DepartmentId,
                Passport = model.Passport == null ? null : PassportModel.ToEntity(model.Passport)
            };
        }
    }
}
