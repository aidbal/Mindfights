using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Migrations
{
    public partial class RenamedMoreModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MindfightAllowedTeams_Mindfights_MindfightId",
                table: "MindfightAllowedTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_MindfightAllowedTeams_Teams_TeamId",
                table: "MindfightAllowedTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_MindfightQuestionAnswers_MindfightQuestions_QuestionId",
                table: "MindfightQuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_MindfightQuestions_Mindfights_MindfightId",
                table: "MindfightQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_MindfightRegistrations_Mindfights_MindfightId",
                table: "MindfightRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_MindfightRegistrations_Teams_TeamId",
                table: "MindfightRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamAnswers_MindfightQuestions_QuestionId",
                table: "TeamAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MindfightRegistrations",
                table: "MindfightRegistrations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MindfightQuestions",
                table: "MindfightQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MindfightQuestionAnswers",
                table: "MindfightQuestionAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MindfightAllowedTeams",
                table: "MindfightAllowedTeams");

            migrationBuilder.RenameTable(
                name: "MindfightRegistrations",
                newName: "Registrations");

            migrationBuilder.RenameTable(
                name: "MindfightQuestions",
                newName: "Questions");

            migrationBuilder.RenameTable(
                name: "MindfightQuestionAnswers",
                newName: "Answers");

            migrationBuilder.RenameTable(
                name: "MindfightAllowedTeams",
                newName: "MindfightConfirmedTeams");

            migrationBuilder.RenameIndex(
                name: "IX_MindfightRegistrations_TeamId",
                table: "Registrations",
                newName: "IX_Registrations_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_MindfightRegistrations_MindfightId",
                table: "Registrations",
                newName: "IX_Registrations_MindfightId");

            migrationBuilder.RenameIndex(
                name: "IX_MindfightQuestions_MindfightId",
                table: "Questions",
                newName: "IX_Questions_MindfightId");

            migrationBuilder.RenameIndex(
                name: "IX_MindfightQuestionAnswers_QuestionId",
                table: "Answers",
                newName: "IX_Answers_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_MindfightAllowedTeams_TeamId",
                table: "MindfightConfirmedTeams",
                newName: "IX_MindfightConfirmedTeams_TeamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Registrations",
                table: "Registrations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Questions",
                table: "Questions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answers",
                table: "Answers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MindfightConfirmedTeams",
                table: "MindfightConfirmedTeams",
                columns: new[] { "MindfightId", "TeamId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MindfightConfirmedTeams_Mindfights_MindfightId",
                table: "MindfightConfirmedTeams",
                column: "MindfightId",
                principalTable: "Mindfights",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MindfightConfirmedTeams_Teams_TeamId",
                table: "MindfightConfirmedTeams",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Mindfights_MindfightId",
                table: "Questions",
                column: "MindfightId",
                principalTable: "Mindfights",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Mindfights_MindfightId",
                table: "Registrations",
                column: "MindfightId",
                principalTable: "Mindfights",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Teams_TeamId",
                table: "Registrations",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAnswers_Questions_QuestionId",
                table: "TeamAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_MindfightConfirmedTeams_Mindfights_MindfightId",
                table: "MindfightConfirmedTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_MindfightConfirmedTeams_Teams_TeamId",
                table: "MindfightConfirmedTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Mindfights_MindfightId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Mindfights_MindfightId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Teams_TeamId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamAnswers_Questions_QuestionId",
                table: "TeamAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Registrations",
                table: "Registrations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Questions",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MindfightConfirmedTeams",
                table: "MindfightConfirmedTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answers",
                table: "Answers");

            migrationBuilder.RenameTable(
                name: "Registrations",
                newName: "MindfightRegistrations");

            migrationBuilder.RenameTable(
                name: "Questions",
                newName: "MindfightQuestions");

            migrationBuilder.RenameTable(
                name: "MindfightConfirmedTeams",
                newName: "MindfightAllowedTeams");

            migrationBuilder.RenameTable(
                name: "Answers",
                newName: "MindfightQuestionAnswers");

            migrationBuilder.RenameIndex(
                name: "IX_Registrations_TeamId",
                table: "MindfightRegistrations",
                newName: "IX_MindfightRegistrations_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_Registrations_MindfightId",
                table: "MindfightRegistrations",
                newName: "IX_MindfightRegistrations_MindfightId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_MindfightId",
                table: "MindfightQuestions",
                newName: "IX_MindfightQuestions_MindfightId");

            migrationBuilder.RenameIndex(
                name: "IX_MindfightConfirmedTeams_TeamId",
                table: "MindfightAllowedTeams",
                newName: "IX_MindfightAllowedTeams_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_QuestionId",
                table: "MindfightQuestionAnswers",
                newName: "IX_MindfightQuestionAnswers_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MindfightRegistrations",
                table: "MindfightRegistrations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MindfightQuestions",
                table: "MindfightQuestions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MindfightAllowedTeams",
                table: "MindfightAllowedTeams",
                columns: new[] { "MindfightId", "TeamId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MindfightQuestionAnswers",
                table: "MindfightQuestionAnswers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MindfightAllowedTeams_Mindfights_MindfightId",
                table: "MindfightAllowedTeams",
                column: "MindfightId",
                principalTable: "Mindfights",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MindfightAllowedTeams_Teams_TeamId",
                table: "MindfightAllowedTeams",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MindfightQuestionAnswers_MindfightQuestions_QuestionId",
                table: "MindfightQuestionAnswers",
                column: "QuestionId",
                principalTable: "MindfightQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MindfightQuestions_Mindfights_MindfightId",
                table: "MindfightQuestions",
                column: "MindfightId",
                principalTable: "Mindfights",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MindfightRegistrations_Mindfights_MindfightId",
                table: "MindfightRegistrations",
                column: "MindfightId",
                principalTable: "Mindfights",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MindfightRegistrations_Teams_TeamId",
                table: "MindfightRegistrations",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAnswers_MindfightQuestions_QuestionId",
                table: "TeamAnswers",
                column: "QuestionId",
                principalTable: "MindfightQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
