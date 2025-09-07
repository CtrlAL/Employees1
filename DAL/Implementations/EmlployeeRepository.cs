using Entity;
using QueryParams;
using System.Data;
using Dapper;
using DAL.Interfaces;

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
                    e.id,
                    e.company_id,
                    e.name,
                    e.surname,
                    e.phone,
                    p.id AS passport_id,
                    p.type,
                    p.number,
                    d.id AS department_id,
                    d.name AS department_name,
                    d.phone AS department_phone
                FROM employees e
                LEFT JOIN passports p ON e.id = p.employee_id
                LEFT JOIN departments d ON e.department_id = d.id
                WHERE e.id = @Id";

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
                splitOn: "passport_id,department_id"
            );

            return employeeDictionary.Values.FirstOrDefault();
        }

        public async Task<IList<Employee>> GetAsync(EmployeesQueryParams query)
        {
            var sql = @"
                SELECT 
                    e.id,
                    e.company_id,
                    e.name,
                    e.surname,
                    e.phone,
                    p.id AS passport_id,
                    p.type,
                    p.number,
                    d.id AS department_id,
                    d.name AS department_name,
                    d.phone AS department_phone
                FROM employees e
                LEFT JOIN passports p ON e.id = p.employee_id
                LEFT JOIN departments d ON e.department_id = d.id
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (query.CompanyId.HasValue)
            {
                sql += " AND e.company_id = @CompanyId";
                parameters.Add("CompanyId", query.CompanyId.Value);
            }

            if (query.DepartmentId.HasValue)
            {
                sql += " AND e.department_id = @DepartmentId";
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
                splitOn: "passport_id,department_id"
            );

            return employeeDictionary.Values.ToList();
        }

        public async Task<int> CreateAsync(Employee employee)
        {
            const string sql = @"
                INSERT INTO employees (company_id, name, surname, phone)
                VALUES (@CompanyId, @Name, @Surname, @Phone)
                RETURNING id;";

            var newId = await _connection.QuerySingleAsync<int>(sql, employee);

            if (employee.Passport != null)
            {
                await _connection.ExecuteAsync(@"
                    INSERT INTO passports (employee_id, type, number)
                    VALUES (@EmployeeId, @Type, @Number);", new
                {
                    EmployeeId = newId,
                    employee.Passport.Type,
                    employee.Passport.Number
                });
            }

            employee.Id = newId;
            return newId;
        }

        public async Task<bool> UpdateAsync(Employee employee)
        {
            const string sql = @"
                UPDATE employees 
                SET company_id = @CompanyId, name = @Name, surname = @Surname, phone = @Phone
                WHERE id = @Id";

            var rowsAffected = await _connection.ExecuteAsync(sql, employee);

            if (rowsAffected == 0)
            {
                return false;
            }

            if (employee.Passport != null)
            {
                var passportUpdateSql = @"
                    UPDATE passports 
                    SET type = @Type, number = @Number
                    WHERE employee_id = @EmployeeId";

                var passportRowsAffected = await _connection.ExecuteAsync(passportUpdateSql, new
                {
                    employee.Passport.Type,
                    employee.Passport.Number,
                    EmployeeId = employee.Id
                });

                if (passportRowsAffected == 0)
                {
                    await _connection.ExecuteAsync(@"
                        INSERT INTO passports (employee_id, type, number)
                        VALUES (@EmployeeId, @Type, @Number);", new
                    {
                        employee.Passport.Type,
                        employee.Passport.Number,
                        EmployeeId = employee.Id
                    });
                }
            }

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _connection.ExecuteAsync("DELETE FROM passports WHERE employee_id = @Id", new { Id = id });
            var rowsAffected = await _connection.ExecuteAsync("DELETE FROM employees WHERE id = @Id", new { Id = id });
            return rowsAffected > 0;
        }
    }
}