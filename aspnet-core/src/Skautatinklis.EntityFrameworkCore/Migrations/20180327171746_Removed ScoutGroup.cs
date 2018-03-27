using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Migrations
{
    public partial class RemovedScoutGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_ScoutGroups_ScoutGroupId",
                table: "AbpUsers");

            migrationBuilder.DropTable(
                name: "ScoutGroups");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_ScoutGroupId",
                table: "AbpUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                    UsersCount = table.Column<int>(nullable: false)
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
    }
}
