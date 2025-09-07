using DAL.Implementations;
using DAL.Implementations.DAL.Implementations;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace DAL.Extensions
{
    public static class Extensions
    {
        public static void UseDal(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDbConnection>(sp =>
            {
                var conn = new NpgsqlConnection(configuration["ConnectionStrings:DefaultConnectionString"]);
                conn.Open();
                return conn;
            });

            services.AddScoped<IEmlployeeRepository, EmployeeRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        }
    }
}
