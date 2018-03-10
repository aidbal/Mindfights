using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Migrations
{
    public partial class Addedanswermodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MindfightQuestionAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    IsCorrect = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindfightQuestionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MindfightQuestionAnswers_MindfightQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "MindfightQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MindfightQuestionAnswers_QuestionId",
                table: "MindfightQuestionAnswers",
                column: "QuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MindfightQuestionAnswers");
        }
    }
}
