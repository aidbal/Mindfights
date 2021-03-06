﻿using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Mindfights.Migrations
{
    public partial class AddedPlaceToResults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mindfights_Teams_WinnersId",
                table: "Mindfights");

            migrationBuilder.DropIndex(
                name: "IX_Mindfights_WinnersId",
                table: "Mindfights");

            migrationBuilder.DropColumn(
                name: "WinnersId",
                table: "Mindfights");

            migrationBuilder.DropColumn(
                name: "IsWinner",
                table: "MindfightResults");

            migrationBuilder.AddColumn<int>(
                name: "Place",
                table: "MindfightResults",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Place",
                table: "MindfightResults");

            migrationBuilder.AddColumn<long>(
                name: "WinnersId",
                table: "Mindfights",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsWinner",
                table: "MindfightResults",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Mindfights_WinnersId",
                table: "Mindfights",
                column: "WinnersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mindfights_Teams_WinnersId",
                table: "Mindfights",
                column: "WinnersId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
