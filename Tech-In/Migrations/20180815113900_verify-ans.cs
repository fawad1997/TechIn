using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Tech_In.Migrations
{
    public partial class verifyans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionVisitor_AspNetUsers_UserId",
                table: "QuestionVisitor");

            migrationBuilder.DropIndex(
                name: "IX_QuestionVisitor_UserId",
                table: "QuestionVisitor");

            migrationBuilder.AddColumn<bool>(
                name: "HasVerifiedAns",
                table: "UserQuestion",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "UserQAnswer",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "QuestionVisitor",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasVerifiedAns",
                table: "UserQuestion");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "UserQAnswer");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "QuestionVisitor",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionVisitor_UserId",
                table: "QuestionVisitor",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionVisitor_AspNetUsers_UserId",
                table: "QuestionVisitor",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
