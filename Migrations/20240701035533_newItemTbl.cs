﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class newItemTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductCategoryName",
                table: "Items",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductCategoryName",
                table: "Items");
        }
    }
}
