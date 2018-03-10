using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Migrations
{
    public partial class Extendedusermodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Birthday",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmedByLeader",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserType",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    GamePoints = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    PlayersCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_TeamId",
                table: "AbpUsers",
                column: "TeamId",
                unique: true,
                filter: "[TeamId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_Team_TeamId",
                table: "AbpUsers",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_Team_TeamId",
                table: "AbpUsers");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_TeamId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Birthday",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "IsConfirmedByLeader",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "AbpUsers");
        }
    }
}
