using Microsoft.EntityFrameworkCore.Migrations;

namespace Pjfm.WebClient.Data.Migrations.Application
{
    public partial class TopTrackApplicationUserCascadingDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TopTracks_AspNetUsers_ApplicationUserId",
                table: "TopTracks");

            migrationBuilder.AddForeignKey(
                name: "FK_TopTracks_AspNetUsers_ApplicationUserId",
                table: "TopTracks",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TopTracks_AspNetUsers_ApplicationUserId",
                table: "TopTracks");

            migrationBuilder.AddForeignKey(
                name: "FK_TopTracks_AspNetUsers_ApplicationUserId",
                table: "TopTracks",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
