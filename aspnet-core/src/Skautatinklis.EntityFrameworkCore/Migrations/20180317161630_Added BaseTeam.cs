using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Migrations
{
    public partial class AddedBaseTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_ScoutGroups_ScoutGroupId",
                table: "AbpUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_ScoutGroups_TeamId",
                table: "AbpUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_MindfightAllowedTeams_ScoutGroups_TeamId",
                table: "MindfightAllowedTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_MindfightRegistrations_ScoutGroups_TeamId",
                table: "MindfightRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_MindfightResults_ScoutGroups_TeamId",
                table: "MindfightResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamAnswers_ScoutGroups_TeamId",
                table: "TeamAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScoutGroups",
                table: "ScoutGroups");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "ScoutGroups");

            migrationBuilder.DropColumn(
                name: "PlayersCount",
                table: "ScoutGroups");

            migrationBuilder.RenameTable(
                name: "ScoutGroups",
                newName: "Teams");

            migrationBuilder.AlterColumn<int>(
                name: "GamePoints",
                table: "Teams",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teams",
                table: "Teams",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ScoutGroups",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LeaderId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoutGroups", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_ScoutGroups_ScoutGroupId",
                table: "AbpUsers",
                column: "ScoutGroupId",
                principalTable: "ScoutGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_Teams_TeamId",
                table: "AbpUsers",
                column: "TeamId",
                principalTable: "Teams",
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
                name: "FK_MindfightRegistrations_Teams_TeamId",
                table: "MindfightRegistrations",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MindfightResults_Teams_TeamId",
                table: "MindfightResults",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAnswers_Teams_TeamId",
                table: "TeamAnswers",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_ScoutGroups_ScoutGroupId",
                table: "AbpUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_Teams_TeamId",
                table: "AbpUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_MindfightAllowedTeams_Teams_TeamId",
                table: "MindfightAllowedTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_MindfightRegistrations_Teams_TeamId",
                table: "MindfightRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_MindfightResults_Teams_TeamId",
                table: "MindfightResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamAnswers_Teams_TeamId",
                table: "TeamAnswers");

            migrationBuilder.DropTable(
                name: "ScoutGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teams",
                table: "Teams");

            migrationBuilder.RenameTable(
                name: "Teams",
                newName: "ScoutGroups");

            migrationBuilder.AlterColumn<int>(
                name: "GamePoints",
                table: "ScoutGroups",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "ScoutGroups",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PlayersCount",
                table: "ScoutGroups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScoutGroups",
                table: "ScoutGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_ScoutGroups_ScoutGroupId",
                table: "AbpUsers",
                column: "ScoutGroupId",
                principalTable: "ScoutGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_ScoutGroups_TeamId",
                table: "AbpUsers",
                column: "TeamId",
                principalTable: "ScoutGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MindfightAllowedTeams_ScoutGroups_TeamId",
                table: "MindfightAllowedTeams",
                column: "TeamId",
                principalTable: "ScoutGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MindfightRegistrations_ScoutGroups_TeamId",
                table: "MindfightRegistrations",
                column: "TeamId",
                principalTable: "ScoutGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MindfightResults_ScoutGroups_TeamId",
                table: "MindfightResults",
                column: "TeamId",
                principalTable: "ScoutGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAnswers_ScoutGroups_TeamId",
                table: "TeamAnswers",
                column: "TeamId",
                principalTable: "ScoutGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
