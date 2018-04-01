using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Mindfights.Migrations
{
    public partial class AddedWinnersProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WonMindfightsCount",
                table: "Teams",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "WinnersId",
                table: "Mindfights",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mindfights_CreatorId",
                table: "Mindfights",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Mindfights_WinnersId",
                table: "Mindfights",
                column: "WinnersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mindfights_AbpUsers_CreatorId",
                table: "Mindfights",
                column: "CreatorId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mindfights_Teams_WinnersId",
                table: "Mindfights",
                column: "WinnersId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mindfights_AbpUsers_CreatorId",
                table: "Mindfights");

            migrationBuilder.DropForeignKey(
                name: "FK_Mindfights_Teams_WinnersId",
                table: "Mindfights");

            migrationBuilder.DropIndex(
                name: "IX_Mindfights_CreatorId",
                table: "Mindfights");

            migrationBuilder.DropIndex(
                name: "IX_Mindfights_WinnersId",
                table: "Mindfights");

            migrationBuilder.DropColumn(
                name: "WonMindfightsCount",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "WinnersId",
                table: "Mindfights");
        }
    }
}
