using Entity;

namespace Employees.Models
{
    public class CompanyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static CompanyModel ToModel(Company entity)
        {
            return new CompanyModel
            {
                Id = entity.Id,
                Name = entity.Name,
            };
        }

        public static Company ToEntity(CompanyModel model)
        {
            return new Company
            {
                Id = model.Id,
                Name = model.Name,
            };
        }
    }
}
