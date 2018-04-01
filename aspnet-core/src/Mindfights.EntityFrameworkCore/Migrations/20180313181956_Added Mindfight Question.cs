using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Mindfights.Migrations
{
    public partial class AddedMindfightQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MindfightQuestions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttachmentLocation = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    MindfightId = table.Column<long>(nullable: false),
                    OrderNumber = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    QuestionTypeId = table.Column<long>(nullable: true),
                    TimeToAnswerInSeconds = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindfightQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MindfightQuestions_Mindfights_MindfightId",
                        column: x => x.MindfightId,
                        principalTable: "Mindfights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MindfightQuestions_MindfightQuestionTypes_QuestionTypeId",
                        column: x => x.QuestionTypeId,
                        principalTable: "MindfightQuestionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MindfightQuestions_MindfightId",
                table: "MindfightQuestions",
                column: "MindfightId");

            migrationBuilder.CreateIndex(
                name: "IX_MindfightQuestions_QuestionTypeId",
                table: "MindfightQuestions",
                column: "QuestionTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MindfightQuestions");
        }
    }
}
