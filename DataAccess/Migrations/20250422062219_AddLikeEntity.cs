using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddLikeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Communities_EntityCommunityId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Communities_EntityCommunityId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Posts_EntityPostId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Users_EntityUserId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Dislikes",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "EntityUserId",
                table: "Images",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "EntityPostId",
                table: "Images",
                newName: "PostId");

            migrationBuilder.RenameColumn(
                name: "EntityCommunityId",
                table: "Images",
                newName: "CommunityId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_EntityUserId",
                table: "Images",
                newName: "IX_Images_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_EntityPostId",
                table: "Images",
                newName: "IX_Images_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_EntityCommunityId",
                table: "Images",
                newName: "IX_Images_CommunityId");

            migrationBuilder.RenameColumn(
                name: "EntityCommunityId",
                table: "Categories",
                newName: "CommunityId");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_EntityCommunityId",
                table: "Categories",
                newName: "IX_Categories_CommunityId");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PostId = table.Column<long>(type: "bigint", nullable: false),
                    IsLike = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Likes_PostId",
                table: "Likes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId",
                table: "Likes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Communities_CommunityId",
                table: "Categories",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Communities_CommunityId",
                table: "Images",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Posts_PostId",
                table: "Images",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Users_UserId",
                table: "Images",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Communities_CommunityId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Communities_CommunityId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Posts_PostId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Users_UserId",
                table: "Images");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Images",
                newName: "EntityUserId");

            migrationBuilder.RenameColumn(
                name: "PostId",
                table: "Images",
                newName: "EntityPostId");

            migrationBuilder.RenameColumn(
                name: "CommunityId",
                table: "Images",
                newName: "EntityCommunityId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_UserId",
                table: "Images",
                newName: "IX_Images_EntityUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_PostId",
                table: "Images",
                newName: "IX_Images_EntityPostId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_CommunityId",
                table: "Images",
                newName: "IX_Images_EntityCommunityId");

            migrationBuilder.RenameColumn(
                name: "CommunityId",
                table: "Categories",
                newName: "EntityCommunityId");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_CommunityId",
                table: "Categories",
                newName: "IX_Categories_EntityCommunityId");

            migrationBuilder.AddColumn<int>(
                name: "Dislikes",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Communities_EntityCommunityId",
                table: "Categories",
                column: "EntityCommunityId",
                principalTable: "Communities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Communities_EntityCommunityId",
                table: "Images",
                column: "EntityCommunityId",
                principalTable: "Communities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Posts_EntityPostId",
                table: "Images",
                column: "EntityPostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Users_EntityUserId",
                table: "Images",
                column: "EntityUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
