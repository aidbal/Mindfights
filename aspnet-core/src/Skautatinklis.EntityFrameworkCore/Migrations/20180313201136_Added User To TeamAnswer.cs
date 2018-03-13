using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Migrations
{
    public partial class AddedUserToTeamAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamAnswers_Mindfights_MindfightId",
                table: "TeamAnswers");

            migrationBuilder.DropIndex(
                name: "IX_TeamAnswers_MindfightId",
                table: "TeamAnswers");

            migrationBuilder.DropColumn(
                name: "MindfightId",
                table: "TeamAnswers");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "TeamAnswers",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "TeamAnswers",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "MindfightId",
                table: "TeamAnswers",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_TeamAnswers_MindfightId",
                table: "TeamAnswers",
                column: "MindfightId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAnswers_Mindfights_MindfightId",
                table: "TeamAnswers",
                column: "MindfightId",
                principalTable: "Mindfights",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
