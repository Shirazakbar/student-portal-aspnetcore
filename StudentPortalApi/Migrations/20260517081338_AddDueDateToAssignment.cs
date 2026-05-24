using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDueDateToAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Assignments",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Assignments");
        }
    }
}
