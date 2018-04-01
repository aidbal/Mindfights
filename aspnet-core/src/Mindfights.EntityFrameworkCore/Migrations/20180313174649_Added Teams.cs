using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Mindfights.Migrations
{
    public partial class AddedTeams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "ScoutGroups",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GamePoints",
                table: "ScoutGroups",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "ScoutGroups");

            migrationBuilder.DropColumn(
                name: "GamePoints",
                table: "ScoutGroups");
        }
    }
}
