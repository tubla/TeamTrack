using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamTrack.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_ProjectId",
                table: "Tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToUserId",
                table: "Tasks",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId_Status",
                table: "Tasks",
                columns: new[] { "ProjectId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignedToUserId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_ProjectId_Status",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId",
                table: "Tasks",
                column: "ProjectId");
        }
    }
}
