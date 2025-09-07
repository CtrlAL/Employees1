using Entity;

namespace Employees.Models
{
    public class DepartmentModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }

        public static DepartmentModel FromEntity(Department entity)
        {
            return new DepartmentModel
            {
                Name = entity.Name,
                Phone = entity.Phone,
            };
        }

        public static Department ToEntity(DepartmentModel model)
        {
            return new Department(0, model.Name, model.Phone);
        }
    }
}
