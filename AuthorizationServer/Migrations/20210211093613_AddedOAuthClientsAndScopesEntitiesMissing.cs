using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthorizationServer.Migrations
{
    public partial class AddedOAuthClientsAndScopesEntitiesMissing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FallbackUri",
                table: "OAuthClients",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "OAuthClients",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FallbackUri",
                table: "OAuthClients");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "OAuthClients");
        }
    }
}
