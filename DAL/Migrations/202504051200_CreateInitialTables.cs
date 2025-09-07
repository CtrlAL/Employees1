using FluentMigrator;

namespace DAL.Migrations
{
    

    namespace MyApp.Migrations
    {
        [Migration(202504051200)]
        public class CreateInitialTables : Migration
        {
            public override void Up()
            {
                Create.Table("companies")
                    .WithColumn("id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("name").AsString(255).NotNullable();

                Create.Table("departments")
                    .WithColumn("id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("name").AsString(255).NotNullable()
                    .WithColumn("phone").AsString(50).Nullable();

                Create.Table("employees")
                    .WithColumn("id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("company_id").AsInt32().NotNullable()
                    .WithColumn("department_id").AsInt32().Nullable()
                    .WithColumn("name").AsString(100).NotNullable()
                    .WithColumn("surname").AsString(100).NotNullable()
                    .WithColumn("phone").AsString(50).Nullable();

                Create.Table("passports")
                    .WithColumn("id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("employee_id").AsInt32().Unique().NotNullable()
                    .WithColumn("type").AsString(50).NotNullable()
                    .WithColumn("number").AsString(50).NotNullable();

                Create.ForeignKey("fk_employees_company")
                    .FromTable("employees").ForeignColumn("company_id")
                    .ToTable("companies").PrimaryColumn("id");

                Create.ForeignKey("fk_employees_department")
                    .FromTable("employees").ForeignColumn("department_id")
                    .ToTable("departments").PrimaryColumn("id");

                Create.ForeignKey("fk_passports_employee")
                    .FromTable("passports").ForeignColumn("employee_id")
                    .ToTable("employees").PrimaryColumn("id");
            }

            public override void Down()
            {
                Delete.ForeignKey("fk_passports_employee").OnTable("passports");
                Delete.ForeignKey("fk_employees_department").OnTable("employees");
                Delete.ForeignKey("fk_employees_company").OnTable("employees");

                Delete.Table("passports");
                Delete.Table("employees");
                Delete.Table("departments");
                Delete.Table("companies");
            }
        }
    }
}
