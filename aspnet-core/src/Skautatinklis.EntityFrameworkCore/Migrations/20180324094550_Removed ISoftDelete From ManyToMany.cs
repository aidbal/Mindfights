using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Migrations
{
    public partial class RemovedISoftDeleteFromManyToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserMindfightResults");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MindfightRegistrations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MindfightEvaluators");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MindfightAllowedTeams");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserMindfightResults",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MindfightRegistrations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MindfightEvaluators",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MindfightAllowedTeams",
                nullable: false,
                defaultValue: false);
        }
    }
}
