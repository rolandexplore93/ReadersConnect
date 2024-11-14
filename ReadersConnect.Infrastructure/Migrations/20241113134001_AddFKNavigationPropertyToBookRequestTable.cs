using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadersConnect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFKNavigationPropertyToBookRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnedDate",
                table: "BookRequests",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "BookRequests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_BookRequests_ApplicationUserId",
                table: "BookRequests",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookRequests_BookId",
                table: "BookRequests",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookRequests_AspNetUsers_ApplicationUserId",
                table: "BookRequests",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookRequests_Books_BookId",
                table: "BookRequests",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookRequests_AspNetUsers_ApplicationUserId",
                table: "BookRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BookRequests_Books_BookId",
                table: "BookRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookRequests_ApplicationUserId",
                table: "BookRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookRequests_BookId",
                table: "BookRequests");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnedDate",
                table: "BookRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "BookRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
