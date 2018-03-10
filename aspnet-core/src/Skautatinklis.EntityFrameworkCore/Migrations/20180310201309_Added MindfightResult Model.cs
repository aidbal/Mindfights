using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Migrations
{
    public partial class AddedMindfightResultModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MindfightResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsEvaluated = table.Column<bool>(nullable: false),
                    MindfightId = table.Column<int>(nullable: true),
                    Points = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindfightResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MindfightResults_Mindfights_MindfightId",
                        column: x => x.MindfightId,
                        principalTable: "Mindfights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MindfightResults_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserMindfightResult",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    MindfightResultId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMindfightResult", x => new { x.UserId, x.MindfightResultId });
                    table.ForeignKey(
                        name: "FK_UserMindfightResult_MindfightResults_MindfightResultId",
                        column: x => x.MindfightResultId,
                        principalTable: "MindfightResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMindfightResult_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_UserMindfightResult_MindfightResultId",
                table: "UserMindfightResult",
                column: "MindfightResultId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMindfightResult");

            migrationBuilder.DropTable(
                name: "MindfightResults");
        }
    }
}
