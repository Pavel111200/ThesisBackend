using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMovieList.Infrastructure.Data.Migrations
{
    public partial class ChangedMovieAndShowRatingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieRatings_AspNetUsers_UserId",
                table: "MovieRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_TVShowRatings_AspNetUsers_UserId",
                table: "TVShowRatings");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "TVShowRatings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "MovieRatings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieRatings_CustomUsers_UserId",
                table: "MovieRatings",
                column: "UserId",
                principalTable: "CustomUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TVShowRatings_CustomUsers_UserId",
                table: "TVShowRatings",
                column: "UserId",
                principalTable: "CustomUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieRatings_CustomUsers_UserId",
                table: "MovieRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_TVShowRatings_CustomUsers_UserId",
                table: "TVShowRatings");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TVShowRatings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "MovieRatings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieRatings_AspNetUsers_UserId",
                table: "MovieRatings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TVShowRatings_AspNetUsers_UserId",
                table: "TVShowRatings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
