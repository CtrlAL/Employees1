using Entity;
using QueryParams;

namespace DAL.Interfaces
{
    public interface IEmlployeeRepository : IRepository<Employee, EmployeesQueryParams, int>
    {
    }
}