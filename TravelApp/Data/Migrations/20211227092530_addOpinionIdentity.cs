using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class addOpinionIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropTable( "Opinions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
