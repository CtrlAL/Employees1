using DAL.Implementations;
using DAL.Interfaces;
using DAL.Migrations;
using FluentMigrator.Runner;
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
            var connectionString = configuration["ConnectionStrings:DefaultConnectionString"];

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            services.AddScoped<IDbConnection>(sp =>
            {
                var conn = new NpgsqlConnection(connectionString);
                conn.Open();
                return conn;
            });

            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb.AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(CreateInitialTables).Assembly)
                .For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole());


            services.AddScoped<IEmlployeeRepository, EmployeeRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        }
    }
}
