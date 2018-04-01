using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Mindfights.Migrations
{
    public partial class Addedmindfights : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MindfightId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Mindfights",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsConfirmed = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsFinished = table.Column<bool>(nullable: false),
                    IsPrivate = table.Column<bool>(nullable: false),
                    PlayersLimit = table.Column<int>(nullable: false),
                    PrepareTime = table.Column<int>(nullable: true),
                    QuestionsCount = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    TotalPoints = table.Column<int>(nullable: false),
                    TotalTimeLimitInMinutes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mindfights", x => x.Id);
                });

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_Mindfights_MindfightId",
                table: "AbpUsers");

            migrationBuilder.DropTable(
                name: "Mindfights");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_MindfightId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "MindfightId",
                table: "AbpUsers");
        }
    }
}
