using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class Testowanko2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable( "Passenger");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
