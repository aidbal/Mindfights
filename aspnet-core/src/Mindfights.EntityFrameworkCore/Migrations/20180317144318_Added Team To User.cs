using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Mindfights.Migrations
{
    public partial class AddedTeamToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_TeamId",
                table: "AbpUsers",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_ScoutGroups_TeamId",
                table: "AbpUsers",
                column: "TeamId",
                principalTable: "ScoutGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_ScoutGroups_TeamId",
                table: "AbpUsers");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_TeamId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "AbpUsers");
        }
    }
}
