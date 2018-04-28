using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Mindfights.Migrations
{
    public partial class RemovedUnusedProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamAnswers_Tours_TourId",
                table: "TeamAnswers");

            migrationBuilder.DropIndex(
                name: "IX_TeamAnswers_TourId",
                table: "TeamAnswers");

            migrationBuilder.DropColumn(
                name: "QuestionsCount",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "TotalPoints",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "GamePoints",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "UsersCount",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "WonMindfightsCount",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "IsCurrentlyEvaluated",
                table: "TeamAnswers");

            migrationBuilder.DropColumn(
                name: "TourId",
                table: "TeamAnswers");

            migrationBuilder.DropColumn(
                name: "AttachmentLocation",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "MindfightStates");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Mindfights");

            migrationBuilder.DropColumn(
                name: "PlayersLimit",
                table: "Mindfights");

            migrationBuilder.DropColumn(
                name: "TotalPoints",
                table: "Mindfights");

            migrationBuilder.DropColumn(
                name: "TotalTimeLimitInMinutes",
                table: "Mindfights");

            migrationBuilder.RenameColumn(
                name: "ToursCount",
                table: "Mindfights",
                newName: "TeamsLimit");

            migrationBuilder.AlterColumn<int>(
                name: "PrepareTime",
                table: "Mindfights",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TeamsLimit",
                table: "Mindfights",
                newName: "ToursCount");

            migrationBuilder.AddColumn<int>(
                name: "QuestionsCount",
                table: "Tours",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalPoints",
                table: "Tours",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GamePoints",
                table: "Teams",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsersCount",
                table: "Teams",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WonMindfightsCount",
                table: "Teams",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCurrentlyEvaluated",
                table: "TeamAnswers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "TourId",
                table: "TeamAnswers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentLocation",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "MindfightStates",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "PrepareTime",
                table: "Mindfights",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Mindfights",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayersLimit",
                table: "Mindfights",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalPoints",
                table: "Mindfights",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalTimeLimitInMinutes",
                table: "Mindfights",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TeamAnswers_TourId",
                table: "TeamAnswers",
                column: "TourId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAnswers_Tours_TourId",
                table: "TeamAnswers",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
