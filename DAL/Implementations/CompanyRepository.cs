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
                const string sql = "SELECT Id, Name FROM Companies WHERE Id = @Id";
                return await _connection.QueryFirstOrDefaultAsync<Company>(sql, new { Id = id });
            }

            public async Task<IList<Company>> GetAsync(object query)
            {
                const string sql = "SELECT Id, Name FROM Companies";
                return (await _connection.QueryAsync<Company>(sql)).AsList();
            }

            public async Task<int> CreateAsync(Company company)
            {
                const string sql = @"
                INSERT INTO Companies (Name)
                VALUES (@Name)
                RETURNING Id;";

                var newId = await _connection.QuerySingleAsync<int>(sql, company);
                company.Id = newId;
                return newId;
            }

            public async Task<bool> UpdateAsync(Company company)
            {
                const string sql = @"
                UPDATE Companies 
                SET Name = @Name 
                WHERE Id = @Id";

                var rowsAffected = await _connection.ExecuteAsync(sql, company);
                return rowsAffected > 0;
            }

            public async Task<bool> DeleteAsync(int id)
            {
                const string sql = "DELETE FROM Companies WHERE Id = @Id";
                var rowsAffected = await _connection.ExecuteAsync(sql, new { Id = id });
                return rowsAffected > 0;
            }
        }
}
