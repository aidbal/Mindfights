using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Mindfights.Migrations
{
    public partial class AddedMindfightResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MindfightResults",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    EarnedPoints = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsEvaluated = table.Column<bool>(nullable: false),
                    MindfightId = table.Column<long>(nullable: false),
                    TeamId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindfightResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MindfightResults_Mindfights_MindfightId",
                        column: x => x.MindfightId,
                        principalTable: "Mindfights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MindfightResults_ScoutGroups_TeamId",
                        column: x => x.TeamId,
                        principalTable: "ScoutGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserMindfightResults",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    MindfightResultId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMindfightResults", x => new { x.UserId, x.MindfightResultId });
                    table.ForeignKey(
                        name: "FK_UserMindfightResults_MindfightResults_MindfightResultId",
                        column: x => x.MindfightResultId,
                        principalTable: "MindfightResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMindfightResults_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MindfightResults_MindfightId",
                table: "MindfightResults",
                column: "MindfightId");

            migrationBuilder.CreateIndex(
                name: "IX_MindfightResults_TeamId",
                table: "MindfightResults",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMindfightResults_MindfightResultId",
                table: "UserMindfightResults",
                column: "MindfightResultId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMindfightResults");

            migrationBuilder.DropTable(
                name: "MindfightResults");
        }
    }
}
