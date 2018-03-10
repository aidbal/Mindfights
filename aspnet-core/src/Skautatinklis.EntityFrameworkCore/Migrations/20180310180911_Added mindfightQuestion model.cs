using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Migrations
{
    public partial class AddedmindfightQuestionmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MindfightQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttachmentLocation = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    QuestionType = table.Column<int>(nullable: false),
                    TimeToAnswerInSeconds = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindfightQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MindfightQuestionMindfight",
                columns: table => new
                {
                    MindfightId = table.Column<int>(nullable: false),
                    MindfightQuestionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindfightQuestionMindfight", x => new { x.MindfightId, x.MindfightQuestionId });
                    table.ForeignKey(
                        name: "FK_MindfightQuestionMindfight_Mindfights_MindfightId",
                        column: x => x.MindfightId,
                        principalTable: "Mindfights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MindfightQuestionMindfight_MindfightQuestions_MindfightQuestionId",
                        column: x => x.MindfightQuestionId,
                        principalTable: "MindfightQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MindfightQuestionMindfight_MindfightQuestionId",
                table: "MindfightQuestionMindfight",
                column: "MindfightQuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MindfightQuestionMindfight");

            migrationBuilder.DropTable(
                name: "MindfightQuestions");
        }
    }
}
