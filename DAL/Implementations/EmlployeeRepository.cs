using DAL.Interfaces;
using Entity;
using QueryParams;
using System.Data;
using Dapper;

namespace DAL.Implementations
{
    public class EmployeeRepository : IEmlployeeRepository
    {
        private readonly IDbConnection _connection;

        public EmployeeRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Employee> GetAsync(int id)
        {
            const string sql = @"
            SELECT 
                e.Id, e.CompanyId, e.Name, e.Surname, e.Phone,
                p.Id AS PassportId, p.Type, p.Number,
                d.Id AS DepartmentId, d.Name AS DepartmentName, d.Phone AS DepartmentPhone
            FROM Employees e
            LEFT JOIN Passports p ON e.Id = p.EmployeeId
            LEFT JOIN Departments d ON e.DepartmentId = d.Id
            WHERE e.Id = @Id";

            var employeeDictionary = new Dictionary<int, Employee>();

            await _connection.QueryAsync<Employee, Passport, Department, Employee>(
                sql,
                (emp, pass, dept) =>
                {
                    if (!employeeDictionary.TryGetValue(emp.Id, out var existingEmp))
                    {
                        existingEmp = emp;
                        existingEmp.Passport = pass;
                        existingEmp.Department = dept;
                        employeeDictionary.Add(emp.Id, existingEmp);
                    }
                    return existingEmp;
                },
                new { Id = id },
                splitOn: "PassportId, DepartmentId"
            );

            return employeeDictionary.Values.FirstOrDefault();
        }

        public async Task<IList<Employee>> GetAsync(EmployeesQueryParams query)
        {
            var sql = @"
            SELECT 
                e.Id, e.CompanyId, e.Name, e.Surname, e.Phone,
                p.Id AS PassportId, p.Type, p.Number,
                d.Id AS DepartmentId, d.Name AS DepartmentName, d.Phone AS DepartmentPhone
            FROM Employees e
            LEFT JOIN Passports p ON e.Id = p.EmployeeId
            LEFT JOIN Departments d ON e.DepartmentId = d.Id
            WHERE 1=1";

            var parameters = new DynamicParameters();

            if (query.CompanyId.HasValue)
            {
                sql += " AND e.CompanyId = @CompanyId";
                parameters.Add("CompanyId", query.CompanyId.Value);
            }

            if (query.DepartmentId.HasValue)
            {
                sql += " AND e.DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", query.DepartmentId.Value);
            }

            var employeeDictionary = new Dictionary<int, Employee>();

            await _connection.QueryAsync<Employee, Passport, Department, Employee>(
                sql,
                (emp, pass, dept) =>
                {
                    if (!employeeDictionary.TryGetValue(emp.Id, out var existingEmp))
                    {
                        existingEmp = emp;
                        existingEmp.Passport = pass;
                        existingEmp.Department = dept;
                        employeeDictionary.Add(emp.Id, existingEmp);
                    }
                    return existingEmp;
                },
                parameters,
                splitOn: "PassportId,DepartmentId"
            );

            return employeeDictionary.Values.ToList();
        }

        public async Task<int> CreateAsync(Employee employee)
        {
            const string sql = @"
            INSERT INTO Employees (CompanyId, Name, Surname, Phone)
            VALUES (@CompanyId, @Name, @Surname, @Phone)
            RETURNING Id;";

            var newId = await _connection.QuerySingleAsync<int>(sql, employee);

            if (employee.Passport != null)
            {
                var passportSql = @"
                INSERT INTO Passports (EmployeeId, Type, Number)
                VALUES (@EmployeeId, @Type, @Number);";

                await _connection.ExecuteAsync(passportSql, new
                {
                    EmployeeId = newId,
                    employee.Passport.Type,
                    employee.Passport.Number
                });
            }

            employee.Id = newId;
            return newId;
        }

        public async Task<int> UpdateAsync(Employee employee)
        {
            const string sql = @"
            UPDATE Employees 
            SET CompanyId = @CompanyId, Name = @Name, Surname = @Surname, Phone = @Phone
            WHERE Id = @Id";

            var rowsAffected = await _connection.ExecuteAsync(sql, employee);

            if (employee.Passport != null)
            {
                var passportSql = @"
                UPDATE Passports 
                SET Type = @Type, Number = @Number
                WHERE EmployeeId = @EmployeeId";

                await _connection.ExecuteAsync(passportSql, new
                {
                    employee.Passport.Type,
                    employee.Passport.Number,
                    EmployeeId = employee.Id
                });
            }

            return employee.Id;
        }

        public async Task<Employee> DeleteAsync(int id)
        {
            var employee = await GetAsync(id);
            if (employee == null) return null;

            
            await _connection.ExecuteAsync("DELETE FROM Passports WHERE EmployeeId = @Id", new { Id = id });

            await _connection.ExecuteAsync("DELETE FROM Employees WHERE Id = @Id", new { Id = id });

            return employee;
        }
    }
}
