using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend_net.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contact_messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ServiceInterest = table.Column<string>(type: "text", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    NeedsAttention = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "profiles",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Summary = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Available = table.Column<bool>(type: "boolean", nullable: false),
                    AvailableText = table.Column<string>(type: "text", nullable: true),
                    Education = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EducationStartYear = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    EducationEndYear = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    PhotoUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    VideoUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LinkedInUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    GitHubUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profiles", x => x.IdUser);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RepositoryUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SkillType = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "stacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Backend = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Frontend = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Database = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cloud = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Conteinerization = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cicd = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "training_exercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    InitialCodeCsharp = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    InitialCodePython = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    InitialCodeJava = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    InitialCodeTypeScript = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    TestCases = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Solution = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    InputExample = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    OutputExample = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ImagesExample = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training_exercises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "work_experiences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Company = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Position = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompanyUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CompanyLogoUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_experiences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "stack_projects",
                columns: table => new
                {
                    IdStack = table.Column<int>(type: "integer", nullable: false),
                    IdProject = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stack_projects", x => new { x.IdStack, x.IdProject });
                    table.ForeignKey(
                        name: "FK_stack_projects_projects_IdProject",
                        column: x => x.IdProject,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stack_projects_stacks_IdStack",
                        column: x => x.IdStack,
                        principalTable: "stacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_projects_Title",
                table: "projects",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stack_projects_IdProject",
                table: "stack_projects",
                column: "IdProject");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contact_messages");

            migrationBuilder.DropTable(
                name: "profiles");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropTable(
                name: "stack_projects");

            migrationBuilder.DropTable(
                name: "training_exercises");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "work_experiences");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "stacks");
        }
    }
}
