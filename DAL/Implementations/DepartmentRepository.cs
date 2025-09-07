using Entity;
using System.Data;
using Dapper;
using DAL.Interfaces;

namespace DAL.Implementations
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly IDbConnection _connection;

        public DepartmentRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Department> GetAsync(int id)
        {
            const string sql = "SELECT id, name, phone FROM departments WHERE id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<Department>(sql, new { Id = id });
        }

        public async Task<IList<Department>> GetAsync(object query)
        {
            const string sql = "SELECT id, name, phone FROM departments";
            return (await _connection.QueryAsync<Department>(sql)).AsList();
        }

        public async Task<int> CreateAsync(Department department)
        {
            const string sql = @"
                INSERT INTO departments (name, phone)
                VALUES (@Name, @Phone)
                RETURNING id;";

            var newId = await _connection.QuerySingleAsync<int>(sql, department);
            department.Id = newId;
            return newId;
        }

        public async Task<bool> UpdateAsync(Department department)
        {
            var setClauses = new List<string>();
            var parameters = new DynamicParameters();

            parameters.Add("Id", department.Id);

            if (department.Name != null)
            {
                setClauses.Add("name = @Name");
                parameters.Add("Name", department.Name);
            }

            if (department.Phone != null)
            {
                setClauses.Add("phone = @Phone");
                parameters.Add("Phone", department.Phone);
            }

            if (setClauses.Count == 0)
                return true;

            var sql = $@"
            UPDATE departments 
            SET {string.Join(", ", setClauses)}
            WHERE id = @Id";

            var rowsAffected = await _connection.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM departments WHERE id = @Id";
            var rowsAffected = await _connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
}