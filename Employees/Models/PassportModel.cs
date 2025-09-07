using Entity;

namespace Employees.Models
{
    public class PassportModel
    {
        public string Type { get; set; }
        public string Number { get; set; }
        public static PassportModel FromEntity(Passport entity)
        {
            return new PassportModel
            {
                Type = entity.Type,
                Number = entity.Number,
            };
        }

        public static Passport ToEntity(PassportModel model)
        {
            return new Passport(0, model.Type, model.Number);
        }
    }
}
