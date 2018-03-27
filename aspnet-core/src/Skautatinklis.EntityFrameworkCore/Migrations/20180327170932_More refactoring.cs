using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Migrations
{
    public partial class Morerefactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MindfightConfirmedTeams");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Mindfights");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Registrations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Registrations");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Mindfights",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "MindfightConfirmedTeams",
                columns: table => new
                {
                    MindfightId = table.Column<long>(nullable: false),
                    TeamId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindfightConfirmedTeams", x => new { x.MindfightId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_MindfightConfirmedTeams_Mindfights_MindfightId",
                        column: x => x.MindfightId,
                        principalTable: "Mindfights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MindfightConfirmedTeams_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MindfightConfirmedTeams_TeamId",
                table: "MindfightConfirmedTeams",
                column: "TeamId");
        }
    }
}
