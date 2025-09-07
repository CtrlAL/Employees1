using Entity;
using System.Data;
using Dapper;
using DAL.Interfaces;

namespace DAL.Implementations
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbConnection _connection;

        public CompanyRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Company> GetAsync(int id)
        {
            const string sql = "SELECT id, name FROM companies WHERE id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<Company>(sql, new { Id = id });
        }

        public async Task<IList<Company>> GetAsync(object query)
        {
            const string sql = "SELECT id, name FROM companies";
            return (await _connection.QueryAsync<Company>(sql)).AsList();
        }

        public async Task<int> CreateAsync(Company company)
        {
            const string sql = @"
                INSERT INTO companies (name)
                VALUES (@Name)
                RETURNING id;";

            var newId = await _connection.QuerySingleAsync<int>(sql, company);
            company.Id = newId;
            return newId;
        }

        public async Task<bool> UpdateAsync(Company company)
        {
            var setClauses = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("Id", company.Id);

            if (company.Name != null)
            {
                setClauses.Add("name = @Name");
                parameters.Add("Name", company.Name);
            }

            if (setClauses.Count == 0)
                return true;

            var sql = $@"
            UPDATE companies 
            SET {string.Join(", ", setClauses)}
            WHERE id = @Id";

            var rowsAffected = await _connection.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM companies WHERE id = @Id";
            var rowsAffected = await _connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
}