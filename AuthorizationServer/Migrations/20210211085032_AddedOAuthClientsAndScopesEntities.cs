using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthorizationServer.Migrations
{
    public partial class AddedOAuthClientsAndScopesEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OAuthClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClientSecret = table.Column<Guid>(type: "TEXT", nullable: false),
                    AppName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OAuthScope",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    OAuthClientsId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthScope", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OAuthScope_OAuthClients_OAuthClientsId",
                        column: x => x.OAuthClientsId,
                        principalTable: "OAuthClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OAuthScope_OAuthClientsId",
                table: "OAuthScope",
                column: "OAuthClientsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OAuthScope");

            migrationBuilder.DropTable(
                name: "OAuthClients");
        }
    }
}
