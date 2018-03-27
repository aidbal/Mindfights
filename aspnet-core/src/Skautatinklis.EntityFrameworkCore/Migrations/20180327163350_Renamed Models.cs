using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Migrations
{
    public partial class RenamedModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MindfightEvaluators");

            migrationBuilder.RenameColumn(
                name: "Answer",
                table: "MindfightQuestionAnswers",
                newName: "Description");

            migrationBuilder.CreateTable(
                name: "MindfightEvaluator",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    MindfightId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindfightEvaluator", x => new { x.UserId, x.MindfightId });
                    table.ForeignKey(
                        name: "FK_MindfightEvaluator_Mindfights_MindfightId",
                        column: x => x.MindfightId,
                        principalTable: "Mindfights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MindfightEvaluator_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MindfightEvaluator_MindfightId",
                table: "MindfightEvaluator",
                column: "MindfightId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MindfightEvaluator");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "MindfightQuestionAnswers",
                newName: "Answer");

            migrationBuilder.CreateTable(
                name: "MindfightEvaluators",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    MindfightId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindfightEvaluators", x => new { x.UserId, x.MindfightId });
                    table.ForeignKey(
                        name: "FK_MindfightEvaluators_Mindfights_MindfightId",
                        column: x => x.MindfightId,
                        principalTable: "Mindfights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MindfightEvaluators_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MindfightEvaluators_MindfightId",
                table: "MindfightEvaluators",
                column: "MindfightId");
        }
    }
}
