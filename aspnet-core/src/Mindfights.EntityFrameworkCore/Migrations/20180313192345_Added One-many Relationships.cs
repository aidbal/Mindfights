using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Mindfights.Migrations
{
    public partial class AddedOnemanyRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_Mindfights_MindfightId",
                table: "AbpUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_ScoutGroups_ScoutGroupId",
                table: "AbpUsers");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_MindfightId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Mindfights");

            migrationBuilder.DropColumn(
                name: "MindfightId",
                table: "AbpUsers");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "TeamAnswers",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Mindfights",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamAnswers_UserId",
                table: "TeamAnswers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Mindfights_UserId",
                table: "Mindfights",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_ScoutGroups_ScoutGroupId",
                table: "AbpUsers",
                column: "ScoutGroupId",
                principalTable: "ScoutGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mindfights_AbpUsers_UserId",
                table: "Mindfights",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAnswers_AbpUsers_UserId",
                table: "TeamAnswers",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_ScoutGroups_ScoutGroupId",
                table: "AbpUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Mindfights_AbpUsers_UserId",
                table: "Mindfights");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamAnswers_AbpUsers_UserId",
                table: "TeamAnswers");

            migrationBuilder.DropIndex(
                name: "IX_TeamAnswers_UserId",
                table: "TeamAnswers");

            migrationBuilder.DropIndex(
                name: "IX_Mindfights_UserId",
                table: "Mindfights");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TeamAnswers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Mindfights");

            migrationBuilder.AddColumn<long>(
                name: "CreatorId",
                table: "Mindfights",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MindfightId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_MindfightId",
                table: "AbpUsers",
                column: "MindfightId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_Mindfights_MindfightId",
                table: "AbpUsers",
                column: "MindfightId",
                principalTable: "Mindfights",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_ScoutGroups_ScoutGroupId",
                table: "AbpUsers",
                column: "ScoutGroupId",
                principalTable: "ScoutGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
