using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Tech_In.Migrations
{
    public partial class usernetwork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserNetwork",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AreFriend = table.Column<bool>(nullable: false),
                    RecordTime = table.Column<DateTime>(nullable: false),
                    User1 = table.Column<string>(nullable: true),
                    User2 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNetwork", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNetwork_AspNetUsers_User1",
                        column: x => x.User1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserNetwork_AspNetUsers_User2",
                        column: x => x.User2,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserNetwork_User1",
                table: "UserNetwork",
                column: "User1");

            migrationBuilder.CreateIndex(
                name: "IX_UserNetwork_User2",
                table: "UserNetwork",
                column: "User2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserNetwork");
        }
    }
}
