﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadersConnect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFieldsInBorrowingRecordTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BookReturnedConfirmedBy",
                table: "BorrowingRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnedDate",
                table: "BorrowingRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnedDate",
                table: "BorrowingRecords");

            migrationBuilder.AlterColumn<string>(
                name: "BookReturnedConfirmedBy",
                table: "BorrowingRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
