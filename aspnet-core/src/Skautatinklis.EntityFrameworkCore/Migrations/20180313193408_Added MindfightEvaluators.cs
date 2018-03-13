using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Migrations
{
    public partial class AddedMindfightEvaluators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mindfights_AbpUsers_UserId",
                table: "Mindfights");

            migrationBuilder.DropIndex(
                name: "IX_Mindfights_UserId",
                table: "Mindfights");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Mindfights");

            migrationBuilder.AddColumn<long>(
                name: "CreatorId",
                table: "Mindfights",
                nullable: false,
                defaultValue: 0L);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MindfightEvaluators");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Mindfights");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Mindfights",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mindfights_UserId",
                table: "Mindfights",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mindfights_AbpUsers_UserId",
                table: "Mindfights",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
