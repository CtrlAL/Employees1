using FluentMigrator;

namespace DAL.Migrations
{
    [Migration(202504051400)]
    public class AddForignKeys : Migration
    {
        public override void Up()
        {
            Alter.Table("departments")
                .AddColumn("company_id").AsInt32();

            Alter.Table("employees")
                .AddColumn("passport_id").AsInt32();

            Create.ForeignKey("fk_departments_company")
                .FromTable("departments").ForeignColumn("company_id")
                .ToTable("companies").PrimaryColumn("id");

            Create.ForeignKey("fk_employees_passport")
                .FromTable("employees").ForeignColumn("passport_id")
                .ToTable("passports").PrimaryColumn("id");
        }

        public override void Down()
        {
            Delete.ForeignKey("fk_employees_passport").OnTable("employees");
            Delete.ForeignKey("fk_departments_company").OnTable("departments");

            Delete.Column("passport_id").FromTable("employees");
            Delete.Column("company_id").FromTable("departments");
        }
    }
}
