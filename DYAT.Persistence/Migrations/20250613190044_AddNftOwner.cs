using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DYAT.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNftOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Nfts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nfts_OwnerId",
                table: "Nfts",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nfts_AspNetUsers_OwnerId",
                table: "Nfts",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nfts_AspNetUsers_OwnerId",
                table: "Nfts");

            migrationBuilder.DropIndex(
                name: "IX_Nfts_OwnerId",
                table: "Nfts");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Nfts");
        }
    }
}
