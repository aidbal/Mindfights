using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Migrations
{
    public partial class ExtendedUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Birthdate",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CityId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_CityId",
                table: "AbpUsers",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_Cities_CityId",
                table: "AbpUsers",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_Cities_CityId",
                table: "AbpUsers");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_CityId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Birthdate",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "AbpUsers");
        }
    }
}
