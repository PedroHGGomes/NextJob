using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextJob.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CANDIDATE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    FullName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ResumeText = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    TechnicalSkills = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SoftSkills = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Certifications = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CurrentRole = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CANDIDATE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JOB_OPENING",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Title = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Company = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    RequiredSkills = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DesiredSkills = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SoftSkills = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Seniority = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    MinSalary = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: true),
                    MaxSalary = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOB_OPENING", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MATCH_RESULT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CandidateId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    JobOpeningId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    RequiredSkillsScore = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    DesiredSkillsScore = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    SoftSkillsScore = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    TotalCompatibility = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    ResumeSuggestions = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    MissingSkills = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    RecommendedCourses = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CareerPlan = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MATCH_RESULT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MATCH_RESULT_CANDIDATE_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "CANDIDATE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MATCH_RESULT_JOB_OPENING_JobOpeningId",
                        column: x => x.JobOpeningId,
                        principalTable: "JOB_OPENING",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MATCH_RESULT_CandidateId",
                table: "MATCH_RESULT",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_MATCH_RESULT_JobOpeningId",
                table: "MATCH_RESULT",
                column: "JobOpeningId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MATCH_RESULT");

            migrationBuilder.DropTable(
                name: "CANDIDATE");

            migrationBuilder.DropTable(
                name: "JOB_OPENING");
        }
    }
}
