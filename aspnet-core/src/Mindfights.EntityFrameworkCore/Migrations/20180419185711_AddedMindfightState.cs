using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Mindfights.Migrations
{
    public partial class AddedMindfightState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IntroTimeInSeconds",
                table: "Tours",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MindfightStates",
                columns: table => new
                {
                    TeamId = table.Column<long>(nullable: false),
                    MindfightId = table.Column<long>(nullable: false),
                    ChangeTime = table.Column<DateTime>(nullable: false),
                    CurrentQuestionId = table.Column<long>(nullable: true),
                    CurrentTourId = table.Column<long>(nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindfightStates", x => new { x.TeamId, x.MindfightId });
                    table.ForeignKey(
                        name: "FK_MindfightStates_Mindfights_MindfightId",
                        column: x => x.MindfightId,
                        principalTable: "Mindfights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MindfightStates_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MindfightStates_MindfightId",
                table: "MindfightStates",
                column: "MindfightId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MindfightStates");

            migrationBuilder.DropColumn(
                name: "IntroTimeInSeconds",
                table: "Tours");
        }
    }
}
