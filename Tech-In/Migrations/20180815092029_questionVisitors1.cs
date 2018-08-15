using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Tech_In.Migrations
{
    public partial class questionVisitors1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionVisitor",
                columns: table => new
                {
                    QuestionVisitorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsLoggedIn = table.Column<bool>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    UserIp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionVisitor", x => x.QuestionVisitorId);
                    table.ForeignKey(
                        name: "FK_QuestionVisitor_UserQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "UserQuestion",
                        principalColumn: "UserQuestionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionVisitor_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionVisitor_QuestionId",
                table: "QuestionVisitor",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionVisitor_UserId",
                table: "QuestionVisitor",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionVisitor");
        }
    }
}
