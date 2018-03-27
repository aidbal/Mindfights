using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Migrations
{
    public partial class AddedTours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Mindfights_MindfightId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamAnswers_AbpUsers_UserId",
                table: "TeamAnswers");

            migrationBuilder.DropIndex(
                name: "IX_TeamAnswers_UserId",
                table: "TeamAnswers");

            migrationBuilder.DropColumn(
                name: "ElapsedTimeInSeconds",
                table: "TeamAnswers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TeamAnswers");

            migrationBuilder.DropColumn(
                name: "ScoutGroupId",
                table: "AbpUsers");

            migrationBuilder.RenameColumn(
                name: "MindfightId",
                table: "Questions",
                newName: "TourId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_MindfightId",
                table: "Questions",
                newName: "IX_Questions_TourId");

            migrationBuilder.AddColumn<long>(
                name: "TourId",
                table: "TeamAnswers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    MindfightId = table.Column<long>(nullable: false),
                    OrderNumber = table.Column<int>(nullable: false),
                    QuestionsCount = table.Column<int>(nullable: false),
                    TimeToEnterAnswersInSeconds = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    TotalPoints = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tours_Mindfights_MindfightId",
                        column: x => x.MindfightId,
                        principalTable: "Mindfights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamAnswers_TourId",
                table: "TeamAnswers",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_MindfightId",
                table: "Tours",
                column: "MindfightId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Tours_TourId",
                table: "Questions",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAnswers_Tours_TourId",
                table: "TeamAnswers",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Tours_TourId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamAnswers_Tours_TourId",
                table: "TeamAnswers");

            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropIndex(
                name: "IX_TeamAnswers_TourId",
                table: "TeamAnswers");

            migrationBuilder.DropColumn(
                name: "TourId",
                table: "TeamAnswers");

            migrationBuilder.RenameColumn(
                name: "TourId",
                table: "Questions",
                newName: "MindfightId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_TourId",
                table: "Questions",
                newName: "IX_Questions_MindfightId");

            migrationBuilder.AddColumn<int>(
                name: "ElapsedTimeInSeconds",
                table: "TeamAnswers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "TeamAnswers",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ScoutGroupId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamAnswers_UserId",
                table: "TeamAnswers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Mindfights_MindfightId",
                table: "Questions",
                column: "MindfightId",
                principalTable: "Mindfights",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAnswers_AbpUsers_UserId",
                table: "TeamAnswers",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
