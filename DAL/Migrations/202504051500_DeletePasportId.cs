using FluentMigrator;

namespace DAL.Migrations
{
    [Migration(202504051500)]
    public class DeletePasportId : Migration
    {
        public override void Up()
        {
            Delete.Column("passport_id").FromTable("employees");
        }

        public override void Down()
        {
            Create.ForeignKey("fk_employees_passport")
                .FromTable("employees").ForeignColumn("passport_id")
                .ToTable("passports").PrimaryColumn("id");
        }
    }
}
