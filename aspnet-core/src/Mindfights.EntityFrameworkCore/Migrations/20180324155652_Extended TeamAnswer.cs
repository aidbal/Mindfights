using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Mindfights.Migrations
{
    public partial class ExtendedTeamAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EarnedPoints",
                table: "TeamAnswers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "EvaluatorId",
                table: "TeamAnswers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamAnswers_EvaluatorId",
                table: "TeamAnswers",
                column: "EvaluatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAnswers_AbpUsers_EvaluatorId",
                table: "TeamAnswers",
                column: "EvaluatorId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamAnswers_AbpUsers_EvaluatorId",
                table: "TeamAnswers");

            migrationBuilder.DropIndex(
                name: "IX_TeamAnswers_EvaluatorId",
                table: "TeamAnswers");

            migrationBuilder.DropColumn(
                name: "EarnedPoints",
                table: "TeamAnswers");

            migrationBuilder.DropColumn(
                name: "EvaluatorId",
                table: "TeamAnswers");
        }
    }
}
