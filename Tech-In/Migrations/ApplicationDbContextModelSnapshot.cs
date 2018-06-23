﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using Tech_In.Data;
using Tech_In.Models.Model;

namespace Tech_In.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Tech_In.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Tech_In.Models.City", b =>
                {
                    b.Property<int>("CityId");

                    b.Property<string>("CityName")
                        .HasMaxLength(50);

                    b.Property<int>("CountryId");

                    b.HasKey("CityId");

                    b.HasIndex("CountryId");

                    b.ToTable("City");
                });

            modelBuilder.Entity("Tech_In.Models.Country", b =>
                {
                    b.Property<int>("CountryId");

                    b.Property<string>("CountryCode")
                        .IsRequired()
                        .HasMaxLength(3);

                    b.Property<string>("CountryName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("CountryPhoneCode")
                        .HasMaxLength(5);

                    b.HasKey("CountryId");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("Tech_In.Models.Database.QuestionSkill", b =>
                {
                    b.Property<int>("QuestionSkillId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("SkillTagId");

                    b.Property<int>("UserQuestionId");

                    b.HasKey("QuestionSkillId");

                    b.HasIndex("SkillTagId");

                    b.HasIndex("UserQuestionId");

                    b.ToTable("QuestionSkill");
                });

            modelBuilder.Entity("Tech_In.Models.Database.SkillTag", b =>
                {
                    b.Property<int>("SkillTagId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("ApprovedStatus");

                    b.Property<string>("SkillName")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<DateTime>("TimeApproved");

                    b.Property<string>("UserId");

                    b.HasKey("SkillTagId");

                    b.HasIndex("UserId");

                    b.ToTable("SkillTag");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserAcheivement", b =>
                {
                    b.Property<int>("UserAchievementId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(70);

                    b.Property<string>("UserId");

                    b.HasKey("UserAchievementId");

                    b.HasIndex("UserId");

                    b.ToTable("UserAcheivement");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserCertification", b =>
                {
                    b.Property<int>("UserCertificationId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CertificationDate");

                    b.Property<DateTime>("ExpirationDate");

                    b.Property<string>("LiscenceNo")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("URL")
                        .HasMaxLength(200);

                    b.Property<string>("UserId");

                    b.HasKey("UserCertificationId");

                    b.HasIndex("UserId");

                    b.ToTable("UserCertification");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserHobby", b =>
                {
                    b.Property<int>("UserHobbyId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("HobbyOrIntrest")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("UserId");

                    b.HasKey("UserHobbyId");

                    b.HasIndex("UserId");

                    b.ToTable("UserHobby");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserLanguageSkill", b =>
                {
                    b.Property<int>("LanguageSkillId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("SkillName")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("UserId");

                    b.HasKey("LanguageSkillId");

                    b.HasIndex("UserId");

                    b.ToTable("UserLanguageSkill");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserPublication", b =>
                {
                    b.Property<int>("UserPublicationId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("ConferenceOrJournal");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<DateTime>("PublishYear");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("UserId");

                    b.HasKey("UserPublicationId");

                    b.HasIndex("UserId");

                    b.ToTable("UserPublication");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserQAComment", b =>
                {
                    b.Property<int>("UserQACommentID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<bool>("IsAnswer");

                    b.Property<string>("UserId");

                    b.Property<int?>("UserQAnswerId");

                    b.Property<int?>("UserQuestionId");

                    b.Property<bool>("Visibility");

                    b.HasKey("UserQACommentID");

                    b.HasIndex("UserId");

                    b.HasIndex("UserQAnswerId");

                    b.HasIndex("UserQuestionId");

                    b.ToTable("UserQAComment");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserQAnswer", b =>
                {
                    b.Property<int>("UserQAnswerId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime>("PostTime");

                    b.Property<string>("UserId");

                    b.Property<int>("UserQuestionId");

                    b.HasKey("UserQAnswerId");

                    b.HasIndex("UserId");

                    b.HasIndex("UserQuestionId");

                    b.ToTable("UserQAnswer");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserQAVoting", b =>
                {
                    b.Property<int>("UserQAVotingID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsAnswer");

                    b.Property<string>("UserId");

                    b.Property<int?>("UserQAnswerId");

                    b.Property<int?>("UserQuestionId");

                    b.Property<int>("Value");

                    b.Property<bool>("Visibility");

                    b.HasKey("UserQAVotingID");

                    b.HasIndex("UserId");

                    b.HasIndex("UserQAnswerId");

                    b.HasIndex("UserQuestionId");

                    b.ToTable("UserQAVoting");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserQuestion", b =>
                {
                    b.Property<int>("UserQuestionId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<DateTime>("PostTime");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("UserId");

                    b.HasKey("UserQuestionId");

                    b.HasIndex("UserId");

                    b.ToTable("UserQuestion");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserSkill", b =>
                {
                    b.Property<int>("UserSkillID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("SkillTagId");

                    b.Property<string>("UserId");

                    b.HasKey("UserSkillID");

                    b.HasIndex("SkillTagId");

                    b.HasIndex("UserId");

                    b.ToTable("UserSkill");
                });

            modelBuilder.Entity("Tech_In.Models.Model.UserEducation", b =>
                {
                    b.Property<int>("UserEducationId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CityId");

                    b.Property<bool>("CurrentStatusCheck");

                    b.Property<string>("Details")
                        .HasMaxLength(200);

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("SchoolName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("UserId")
                        .HasMaxLength(450);

                    b.HasKey("UserEducationId");

                    b.HasIndex("CityId");

                    b.HasIndex("UserId");

                    b.ToTable("UserEducation");
                });

            modelBuilder.Entity("Tech_In.Models.Model.UserExperience", b =>
                {
                    b.Property<int>("UserExperienceId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CityID");

                    b.Property<string>("CompanyName")
                        .HasMaxLength(100);

                    b.Property<bool>("CurrentWorkCheck");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<DateTime>("EndDate");

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("UserId");

                    b.HasKey("UserExperienceId");

                    b.HasIndex("CityID");

                    b.HasIndex("UserId");

                    b.ToTable("UserExperience");
                });

            modelBuilder.Entity("Tech_In.Models.Model.UserPersonalDetail", b =>
                {
                    b.Property<int>("UserPersonalDetailId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<int>("CityId");

                    b.Property<string>("CoverImage");

                    b.Property<DateTime>("DOB");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("Gender");

                    b.Property<bool>("IsDOBPublic");

                    b.Property<bool>("IsEmailPublic");

                    b.Property<bool>("IsPhonePublic");

                    b.Property<string>("LastName")
                        .HasMaxLength(100);

                    b.Property<string>("ProfileImage");

                    b.Property<string>("Summary")
                        .HasMaxLength(300);

                    b.Property<string>("UserId");

                    b.HasKey("UserPersonalDetailId");

                    b.HasIndex("CityId");

                    b.HasIndex("UserId");

                    b.ToTable("UserPersonalDetail");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Tech_In.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Tech_In.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tech_In.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Tech_In.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tech_In.Models.City", b =>
                {
                    b.HasOne("Tech_In.Models.Country", "Country")
                        .WithMany("Cities")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tech_In.Models.Database.QuestionSkill", b =>
                {
                    b.HasOne("Tech_In.Models.Database.SkillTag", "SkillTag")
                        .WithMany()
                        .HasForeignKey("SkillTagId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tech_In.Models.Database.UserQuestion", "UserQuestion")
                        .WithMany()
                        .HasForeignKey("UserQuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tech_In.Models.Database.SkillTag", b =>
                {
                    b.HasOne("Tech_In.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserAcheivement", b =>
                {
                    b.HasOne("Tech_In.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("UserAcheivements")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserCertification", b =>
                {
                    b.HasOne("Tech_In.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("UserCertifications")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserHobby", b =>
                {
                    b.HasOne("Tech_In.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("UserHobbies")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserLanguageSkill", b =>
                {
                    b.HasOne("Tech_In.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("UserLanguageSkills")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserPublication", b =>
                {
                    b.HasOne("Tech_In.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("UserPublications")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserQAComment", b =>
                {
                    b.HasOne("Tech_In.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("UserQAComments")
                        .HasForeignKey("UserId");

                    b.HasOne("Tech_In.Models.Database.UserQAnswer", "UserQAnswer")
                        .WithMany()
                        .HasForeignKey("UserQAnswerId");

                    b.HasOne("Tech_In.Models.Database.UserQuestion", "UserQuestion")
                        .WithMany()
                        .HasForeignKey("UserQuestionId");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserQAnswer", b =>
                {
                    b.HasOne("Tech_In.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("UserQAnswers")
                        .HasForeignKey("UserId");

                    b.HasOne("Tech_In.Models.Database.UserQuestion", "UserQuestion")
                        .WithMany()
                        .HasForeignKey("UserQuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserQAVoting", b =>
                {
                    b.HasOne("Tech_In.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("UserQAVotings")
                        .HasForeignKey("UserId");

                    b.HasOne("Tech_In.Models.Database.UserQAnswer", "UserQAnswer")
                        .WithMany()
                        .HasForeignKey("UserQAnswerId");

                    b.HasOne("Tech_In.Models.Database.UserQuestion", "UserQuestion")
                        .WithMany()
                        .HasForeignKey("UserQuestionId");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserQuestion", b =>
                {
                    b.HasOne("Tech_In.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("Questions")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Tech_In.Models.Database.UserSkill", b =>
                {
                    b.HasOne("Tech_In.Models.Database.SkillTag", "SkillTag")
                        .WithMany()
                        .HasForeignKey("SkillTagId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tech_In.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("UserSkill")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Tech_In.Models.Model.UserEducation", b =>
                {
                    b.HasOne("Tech_In.Models.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tech_In.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("UserEducations")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Tech_In.Models.Model.UserExperience", b =>
                {
                    b.HasOne("Tech_In.Models.City", "City")
                        .WithMany()
                        .HasForeignKey("CityID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tech_In.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("UserExperiences")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Tech_In.Models.Model.UserPersonalDetail", b =>
                {
                    b.HasOne("Tech_In.Models.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tech_In.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("UserPersonalDetails")
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
