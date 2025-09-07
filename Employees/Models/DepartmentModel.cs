using Entity;

namespace Employees.Models
{
    public class DepartmentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        public static DepartmentModel ToModel(Department entity)
        {
            return new DepartmentModel
            {
                Name = entity.Name,
                Phone = entity.Phone,
            };
        }

        public static Department ToEntity(DepartmentModel model)
        {
            return new Department(model.Id, model.Name, model.Phone);
        }
    }
}
