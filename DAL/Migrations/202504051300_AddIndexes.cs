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
                    .OnTable("employees")
                    .OnColumn("company_id");

                Create.Index("idx_employees_department_id")
                    .OnTable("employees")
                    .OnColumn("department_id");

                Create.Index("idx_passports_employee_id")
                    .OnTable("passports")
                    .OnColumn("employee_id")
                    .Unique();

                Create.Index("idx_companies_name")
                    .OnTable("companies")
                    .OnColumn("name");
            }

            public override void Down()
            {
                Delete.Index("idx_companies_name").OnTable("companies");
                Delete.Index("idx_passports_employee_id").OnTable("passports");
                Delete.Index("idx_employees_department_id").OnTable("employees");
                Delete.Index("idx_employees_company_id").OnTable("employees");
            }
        }
    }
}
