using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MergeMaster.Api.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Reports",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                Repository = table.Column<string>(type: "TEXT", nullable: false),
                Branch = table.Column<string>(type: "TEXT", nullable: false),
                CommitSha = table.Column<string>(type: "TEXT", nullable: false),
                CommitMessage = table.Column<string>(type: "TEXT", nullable: true),
                CommitDateUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                AuthorName = table.Column<string>(type: "TEXT", nullable: false),
                AuthorEmail = table.Column<string>(type: "TEXT", nullable: false),
                Urgent = table.Column<bool>(type: "INTEGER", nullable: false),
                Tags = table.Column<string>(type: "TEXT", nullable: false),
                Notes = table.Column<string>(type: "TEXT", nullable: true),
                UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                Validated = table.Column<bool>(type: "INTEGER", nullable: false),
                ArchivedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Reports", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Reports_CommitSha",
            table: "Reports",
            column: "CommitSha",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Reports");
    }
}
