using Microsoft.EntityFrameworkCore.Migrations;

namespace Skautatinklis.Migrations
{
    public partial class AddedMindfightAllowedTeams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MindfightAllowedTeams",
                columns: table => new
                {
                    MindfightId = table.Column<long>(nullable: false),
                    TeamId = table.Column<long>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindfightAllowedTeams", x => new { x.MindfightId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_MindfightAllowedTeams_Mindfights_MindfightId",
                        column: x => x.MindfightId,
                        principalTable: "Mindfights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MindfightAllowedTeams_ScoutGroups_TeamId",
                        column: x => x.TeamId,
                        principalTable: "ScoutGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MindfightAllowedTeams_TeamId",
                table: "MindfightAllowedTeams",
                column: "TeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MindfightAllowedTeams");
        }
    }
}
