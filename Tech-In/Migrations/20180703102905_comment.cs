using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Tech_In.Migrations
{
    public partial class comment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserQAComment_UserQAnswer_UserQAnswerId",
                table: "UserQAComment");

            migrationBuilder.DropIndex(
                name: "IX_UserQAComment_UserQAnswerId",
                table: "UserQAComment");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "UserQAComment",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "UserQAComment",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.CreateIndex(
                name: "IX_UserQAComment_UserQAnswerId",
                table: "UserQAComment",
                column: "UserQAnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserQAComment_UserQAnswer_UserQAnswerId",
                table: "UserQAComment",
                column: "UserQAnswerId",
                principalTable: "UserQAnswer",
                principalColumn: "UserQAnswerId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
