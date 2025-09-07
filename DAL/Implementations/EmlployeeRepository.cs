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
                splitOn: "passport_id, department_id"
            );

            return employeeDictionary.Values.ToList();
        }

        public async Task<int> CreateAsync(Employee employee)
        {
            using var transaction = _connection.BeginTransaction();

            try
            {
                const string insertEmployeeSql = @"
                INSERT INTO employees (company_id, department_id, name, surname, phone)
                VALUES (@CompanyId, @DepartmentId, @Name, @Surname, @Phone)
                RETURNING id;";

                var newId = await _connection.QuerySingleAsync<int>(insertEmployeeSql, employee, transaction);

                if (employee.Passport == null)
                    throw new InvalidOperationException("Passport is required.");

                await _connection.ExecuteAsync(@"
                INSERT INTO passports (employee_id, type, number)
                VALUES (@EmployeeId, @Type, @Number);", new
                {
                    EmployeeId = newId,
                    Type = employee.Passport.Type,
                    Number = employee.Passport.Number
                }, transaction);

                employee.Id = newId;

                transaction.Commit();
                return newId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Employee employee)
        {
            var rowsAffected = 0;

            if (employee.Id <= 0)
                return false;

            var setClauses = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("Id", employee.Id);

            if (employee.CompanyId > 0)
            {
                setClauses.Add("company_id = @CompanyId");
                parameters.Add("CompanyId", employee.CompanyId);
            }

            if (!string.IsNullOrWhiteSpace(employee.Name))
            {
                setClauses.Add("name = @Name");
                parameters.Add("Name", employee.Name);
            }

            if (!string.IsNullOrWhiteSpace(employee.Surname))
            {
                setClauses.Add("surname = @Surname");
                parameters.Add("Surname", employee.Surname);
            }

            if (employee.Phone != null)
            {
                setClauses.Add("phone = @Phone");
                parameters.Add("Phone", employee.Phone);
            }

            if (setClauses.Count == 0 
                && employee.Passport == null 
                && employee.Department == null)
            {
                return true;
            }

            var sql = $@"
            UPDATE employees 
            SET {string.Join(", ", setClauses)}
            WHERE id = @Id";

            rowsAffected += await _connection.ExecuteAsync(sql, parameters);
            rowsAffected += await UpsertPassportAsync(employee.Id, employee.Passport);
            rowsAffected += await UpdateDepartmentAsync(employee.Id, employee.Department);

            if (rowsAffected == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<int> UpsertPassportAsync(int employeeId, Passport? passport)
        {
            if (passport == null)
                return 0;

            const string updateSql = @"
            UPDATE passports 
            SET 
                type = CASE WHEN @Type IS NOT NULL THEN @Type ELSE type END,
                number = CASE WHEN @Number IS NOT NULL THEN @Number ELSE number END
            WHERE employee_id = @EmployeeId";

            var rowsAffected = await _connection.ExecuteAsync(updateSql, new
            {
                Type = passport.Type,
                Number = passport.Number,
                EmployeeId = employeeId
            });

            if (rowsAffected == 0)
            {
                const string insertSql = @"
                INSERT INTO passports (employee_id, type, number)
                VALUES (@EmployeeId, @Type, @Number)";

                return await _connection.ExecuteAsync(insertSql, new
                {
                    Type = passport.Type,
                    Number = passport.Number,
                    EmployeeId = employeeId
                });
            }

            return rowsAffected;
        }

        public async Task<int> UpdateDepartmentAsync(int employeeId, Department? department)
        {
            if (department == null)
                return 0;

            var currentDepartmentId = await _connection.QueryFirstOrDefaultAsync<int?>(@"
            SELECT department_id 
            FROM employees 
            WHERE id = @EmployeeId", new { EmployeeId = employeeId });

            if (!currentDepartmentId.HasValue)
                throw new InvalidOperationException($"Employee {employeeId} is not assigned to any department.");

            const string updateSql = @"
            UPDATE departments 
            SET 
                name = CASE WHEN @Name IS NOT NULL THEN @Name ELSE name END,
                phone = CASE WHEN @Phone IS NOT NULL THEN @Phone ELSE phone END
            WHERE id = @DepartmentId";

            var rowsAffected = await _connection.ExecuteAsync(updateSql, new
            {
                Name = department.Name,
                Phone = department.Phone,
                DepartmentId = currentDepartmentId.Value
            });

            if (rowsAffected == 0)
                throw new InvalidOperationException($"Department with ID {currentDepartmentId} not found.");

            return rowsAffected;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _connection.ExecuteAsync("DELETE FROM passports WHERE employee_id = @Id", new { Id = id });
            var rowsAffected = await _connection.ExecuteAsync("DELETE FROM employees WHERE id = @Id", new { Id = id });
            return rowsAffected > 0;
        }
    }
}