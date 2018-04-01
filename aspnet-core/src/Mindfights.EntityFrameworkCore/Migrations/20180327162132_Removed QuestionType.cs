using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Mindfights.Migrations
{
    public partial class RemovedQuestionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MindfightQuestions_MindfightQuestionTypes_QuestionTypeId",
                table: "MindfightQuestions");

            migrationBuilder.DropTable(
                name: "MindfightQuestionTypes");

            migrationBuilder.DropIndex(
                name: "IX_MindfightQuestions_QuestionTypeId",
                table: "MindfightQuestions");

            migrationBuilder.DropColumn(
                name: "QuestionTypeId",
                table: "MindfightQuestions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "QuestionTypeId",
                table: "MindfightQuestions",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MindfightQuestionTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindfightQuestionTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MindfightQuestions_QuestionTypeId",
                table: "MindfightQuestions",
                column: "QuestionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MindfightQuestions_MindfightQuestionTypes_QuestionTypeId",
                table: "MindfightQuestions",
                column: "QuestionTypeId",
                principalTable: "MindfightQuestionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
