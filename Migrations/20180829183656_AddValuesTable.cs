using Microsoft.EntityFrameworkCore.Migrations;

namespace FinderApp.API.Migrations
{
    public partial class AddValuesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.Sql("INSERT INTO Values (Name) VALUES ('Value1')");
            // migrationBuilder.Sql("INSERT INTO Values (Name) VALUES ('Value2')");
            // migrationBuilder.Sql("INSERT INTO Values (Name) VALUES ('Value3')");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.Sql("DELETE FROM Values WHERE Name IN ('Value1', 'Value2', 'Value3')");

        }
    }
}
