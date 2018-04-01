using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Mindfights.Migrations
{
    public partial class AddedTeamAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamAnswers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    ElapsedTimeInSeconds = table.Column<int>(nullable: false),
                    EnteredAnswer = table.Column<string>(nullable: true),
                    IsCurrentlyEvaluated = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsEvaluated = table.Column<bool>(nullable: false),
                    MindfightId = table.Column<long>(nullable: false),
                    QuestionId = table.Column<long>(nullable: false),
                    TeamId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamAnswers_Mindfights_MindfightId",
                        column: x => x.MindfightId,
                        principalTable: "Mindfights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamAnswers_MindfightQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "MindfightQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamAnswers_ScoutGroups_TeamId",
                        column: x => x.TeamId,
                        principalTable: "ScoutGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamAnswers_MindfightId",
                table: "TeamAnswers",
                column: "MindfightId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAnswers_QuestionId",
                table: "TeamAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAnswers_TeamId",
                table: "TeamAnswers",
                column: "TeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamAnswers");
        }
    }
}
