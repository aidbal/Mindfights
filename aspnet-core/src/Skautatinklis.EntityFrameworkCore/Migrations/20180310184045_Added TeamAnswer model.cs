using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Migrations
{
    public partial class AddedTeamAnswermodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    ElapsedTimeInSeconds = table.Column<int>(nullable: false),
                    EnteredAnswer = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsEvaluated = table.Column<bool>(nullable: false),
                    MindfightId = table.Column<int>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamAnswers_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
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
