using Entity;

namespace Employees.Models.CreateModels
{
    public class CreateDepartmentModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        public static Department ToEntity(CreateDepartmentModel model)
        {
            return new Department
            {
                Id = model.Id,
                CompanyId = model.CompanyId,
                Name = model.Name,
                Phone = model.Phone,
            };
        }
    }
}
