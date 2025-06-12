using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quiz_application.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserQuizAttemptTimeProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClientStartTime",
                table: "UserQuizAttempts",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "ClientEndTime",
                table: "UserQuizAttempts",
                newName: "EndTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "UserQuizAttempts",
                newName: "ClientStartTime");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "UserQuizAttempts",
                newName: "ClientEndTime");
        }
    }
}
