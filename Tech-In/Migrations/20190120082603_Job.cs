using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Tech_In.Migrations
{
    public partial class Job : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    About = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    FoundedDate = table.Column<DateTime>(nullable: false),
                    Location = table.Column<int>(nullable: false),
                    Logo = table.Column<string>(nullable: true),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Type = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    WebSite = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Company_City_Location",
                        column: x => x.Location,
                        principalTable: "City",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Company_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Job",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    JobShift = table.Column<int>(nullable: false),
                    JobType = table.Column<int>(nullable: false),
                    Location = table.Column<int>(nullable: false),
                    MaxExpereince = table.Column<int>(nullable: false),
                    MaxSalary = table.Column<int>(nullable: false),
                    MinExpereince = table.Column<int>(nullable: false),
                    MinSalary = table.Column<int>(nullable: false),
                    PostDate = table.Column<DateTime>(nullable: false),
                    PostedBy = table.Column<int>(nullable: true),
                    Qualification = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    vacancies = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job_City_Location",
                        column: x => x.Location,
                        principalTable: "City",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Job_Company_PostedBy",
                        column: x => x.PostedBy,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Applicant",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AppliedBy = table.Column<string>(nullable: true),
                    JobId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applicant_AspNetUsers_AppliedBy",
                        column: x => x.AppliedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applicant_Job_JobId",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobCatagory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActiveJobs = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    JobId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCatagory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobCatagory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobCatagory_Job_JobId",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSkill",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActiveJobs = table.Column<int>(nullable: false),
                    JobId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSkill_Job_JobId",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSkill_SkillTag_TagId",
                        column: x => x.TagId,
                        principalTable: "SkillTag",
                        principalColumn: "SkillTagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedJob",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Job = table.Column<int>(nullable: false),
                    User = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedJob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedJob_Job_Job",
                        column: x => x.Job,
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedJob_AspNetUsers_User",
                        column: x => x.User,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applicant_AppliedBy",
                table: "Applicant",
                column: "AppliedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Applicant_JobId",
                table: "Applicant",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_Location",
                table: "Company",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Company_UserId",
                table: "Company",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Location",
                table: "Job",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Job_PostedBy",
                table: "Job",
                column: "PostedBy");

            migrationBuilder.CreateIndex(
                name: "IX_JobCatagory_CategoryId",
                table: "JobCatagory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCatagory_JobId",
                table: "JobCatagory",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkill_JobId",
                table: "JobSkill",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkill_TagId",
                table: "JobSkill",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedJob_Job",
                table: "SavedJob",
                column: "Job");

            migrationBuilder.CreateIndex(
                name: "IX_SavedJob_User",
                table: "SavedJob",
                column: "User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applicant");

            migrationBuilder.DropTable(
                name: "JobCatagory");

            migrationBuilder.DropTable(
                name: "JobSkill");

            migrationBuilder.DropTable(
                name: "SavedJob");

            migrationBuilder.DropTable(
                name: "Job");

            migrationBuilder.DropTable(
                name: "Company");
        }
    }
}
