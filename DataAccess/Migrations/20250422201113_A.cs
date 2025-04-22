using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class A : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Communities_CommunityId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CommunityId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CommunityId",
                table: "Categories",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CommunityId",
                table: "Categories",
                column: "CommunityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Communities_CommunityId",
                table: "Categories",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id");
        }
    }
}
