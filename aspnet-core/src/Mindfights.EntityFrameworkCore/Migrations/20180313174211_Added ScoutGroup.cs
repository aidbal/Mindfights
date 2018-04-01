using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Mindfights.Migrations
{
    public partial class AddedScoutGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ScoutGroupId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ScoutGroups",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LeaderId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PlayersCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoutGroups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_ScoutGroupId",
                table: "AbpUsers",
                column: "ScoutGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_ScoutGroups_ScoutGroupId",
                table: "AbpUsers",
                column: "ScoutGroupId",
                principalTable: "ScoutGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_ScoutGroups_ScoutGroupId",
                table: "AbpUsers");

            migrationBuilder.DropTable(
                name: "ScoutGroups");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_ScoutGroupId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "ScoutGroupId",
                table: "AbpUsers");
        }
    }
}
