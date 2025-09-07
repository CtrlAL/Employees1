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
                Create.Table("Сompanies")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("Name").AsString(255).NotNullable();

                Create.Table("Departments")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("Name").AsString(255).NotNullable()
                    .WithColumn("Phone").AsString(50).Nullable();

                Create.Table("Employees")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("CompanyId").AsInt32().NotNullable()
                    .WithColumn("DepartmentId").AsInt32().Nullable()
                    .WithColumn("Name").AsString(100).NotNullable()
                    .WithColumn("Surname").AsString(100).NotNullable()
                    .WithColumn("Phone").AsString(50).Nullable();

                Create.Table("Passports")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("EmployeeId").AsInt32().Unique().NotNullable()
                    .WithColumn("Type").AsString(50).NotNullable()
                    .WithColumn("Number").AsString(50).NotNullable();

                Create.ForeignKey("fk_employees_company")
                    .FromTable("Employees").ForeignColumn("CompanyId")
                    .ToTable("Companies").PrimaryColumn("Id");

                Create.ForeignKey("fk_employees_department")
                    .FromTable("Employees").ForeignColumn("DepartmentId")
                    .ToTable("Departments").PrimaryColumn("Id");

                Create.ForeignKey("fk_passports_employee")
                    .FromTable("Passports").ForeignColumn("EmployeeId")
                    .ToTable("Employees").PrimaryColumn("Id");
            }

            public override void Down()
            {
                Delete.ForeignKey("fk_passports_employee").OnTable("Passports");
                Delete.ForeignKey("fk_employees_department").OnTable("Employees");
                Delete.ForeignKey("fk_employees_company").OnTable("Employees");

                Delete.Table("Passports");
                Delete.Table("Employees");
                Delete.Table("Departments");
                Delete.Table("Companies");
            }
        }
    }
}
