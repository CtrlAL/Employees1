using FluentMigrator;

namespace DAL.Migrations
{
    namespace MyApp.Migrations
    {
        [Migration(202504051300)]
        public class AddIndexes : Migration
        {
            public override void Up()
            {
                Create.Index("idx_employees_company_id")
                    .OnTable("Employees")
                    .OnColumn("CompanyId");

                Create.Index("idx_employees_department_id")
                    .OnTable("Employees")
                    .OnColumn("DepartmentId");

                Create.Index("idx_passports_employee_id")
                    .OnTable("Passports")
                    .OnColumn("EmployeeId")
                    .Unique();

                Create.Index("idx_companies_name")
                    .OnTable("Companies")
                    .OnColumn("Name");
            }

            public override void Down()
            {
                Delete.Index("idx_companies_name").OnTable("Companies");
                Delete.Index("idx_passports_employee_id").OnTable("Passports");
                Delete.Index("idx_employees_department_id").OnTable("Employees");
                Delete.Index("idx_employees_company_id").OnTable("Employees");
            }
        }
    }
}
