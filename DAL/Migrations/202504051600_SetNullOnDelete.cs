using FluentMigrator;
using System.Data;

namespace DAL.Migrations
{
    [Migration(202504051600)]
    public class SetNullOnDelete : Migration
    {
        public override void Up()
        {
            Alter.Table("employees")
                .AlterColumn("company_id")
                .AsInt32()
                .Nullable();

            Alter.Table("departments")
                .AlterColumn("company_id")
                .AsInt32()
                .Nullable();

            Delete.ForeignKey("fk_passports_employee").OnTable("passports");
            Delete.ForeignKey("fk_employees_department").OnTable("employees");
            Delete.ForeignKey("fk_employees_company").OnTable("employees");
            Delete.ForeignKey("fk_departments_company").OnTable("departments");

            Create.ForeignKey("fk_passports_employee")
                .FromTable("passports").ForeignColumn("employee_id")
                .ToTable("employees").PrimaryColumn("id")
                .OnDelete(Rule.Cascade);

            Create.ForeignKey("fk_departments_company")
                .FromTable("departments").ForeignColumn("company_id")
                .ToTable("companies").PrimaryColumn("id")
                .OnDelete(Rule.SetNull);

            Create.ForeignKey("fk_employees_company")
                .FromTable("employees").ForeignColumn("company_id")
                .ToTable("companies").PrimaryColumn("id")
                .OnDelete(Rule.SetNull);

            Create.ForeignKey("fk_employees_department")
                .FromTable("employees").ForeignColumn("department_id")
                .ToTable("departments").PrimaryColumn("id")
                .OnDelete(Rule.SetNull);
        }

        public override void Down()
        {
            Alter.Table("employees")
                .AlterColumn("company_id")
                .AsInt32()
                .NotNullable();

            Alter.Table("departments")
                .AlterColumn("company_id")
                .AsInt32()
                .NotNullable();

            Delete.ForeignKey("fk_passports_employee").OnTable("passports");
            Delete.ForeignKey("fk_employees_department").OnTable("employees");
            Delete.ForeignKey("fk_employees_company").OnTable("employees");
            Delete.ForeignKey("fk_departments_company").OnTable("departments");

            Create.ForeignKey("fk_passports_employee")
                .FromTable("passports").ForeignColumn("employee_id")
                .ToTable("employees").PrimaryColumn("id");

            Create.ForeignKey("fk_departments_company")
                .FromTable("departments").ForeignColumn("company_id")
                .ToTable("companies").PrimaryColumn("id");

            Create.ForeignKey("fk_employees_company")
                .FromTable("employees").ForeignColumn("company_id")
                .ToTable("companies").PrimaryColumn("id");

            Create.ForeignKey("fk_employees_department")
                .FromTable("employees").ForeignColumn("department_id")
                .ToTable("departments").PrimaryColumn("id");
        }
    }
}
