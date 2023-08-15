using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoProject.API.Migrations
{
    /// <inheritdoc />
    public partial class tot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "CardGroups",
                columns: table => new
                {
                    CardGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardGroups", x => x.CardGroupId);
                    table.ForeignKey(
                        name: "FK_CardGroups_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Friendships",
                columns: table => new
                {
                    FriendshipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FUserId = table.Column<int>(type: "int", nullable: false),
                    SUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendships", x => x.FriendshipId);
                    table.ForeignKey(
                        name: "FK_Friendships_Users_FUserId",
                        column: x => x.FUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friendships_Users_SUserId",
                        column: x => x.SUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    UserGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.UserGroupId);
                    table.ForeignKey(
                        name: "FK_UserGroups_Users_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserUserGroup",
                columns: table => new
                {
                    UserGroupsUserGroupId = table.Column<int>(type: "int", nullable: false),
                    UsersUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUserGroup", x => new { x.UserGroupsUserGroupId, x.UsersUserId });
                    table.ForeignKey(
                        name: "FK_UserUserGroup_UserGroups_UserGroupsUserGroupId",
                        column: x => x.UserGroupsUserGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "UserGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserUserGroup_Users_UsersUserId",
                        column: x => x.UsersUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    CardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Info = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardGroupId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Grade = table.Column<int>(type: "int", nullable: true),
                    Completion = table.Column<int>(type: "int", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    PreviousCardId = table.Column<int>(type: "int", nullable: true),
                    NextCardId = table.Column<int>(type: "int", nullable: true),
                    DocFolderId = table.Column<int>(type: "int", nullable: true),
                    LinkFolderId = table.Column<int>(type: "int", nullable: true),
                    ImageFolderId = table.Column<int>(type: "int", nullable: true),
                    CardSequenceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.CardId);
                    table.ForeignKey(
                        name: "FK_Cards_CardGroups_CardGroupId",
                        column: x => x.CardGroupId,
                        principalTable: "CardGroups",
                        principalColumn: "CardGroupId");
                    table.ForeignKey(
                        name: "FK_Cards_Cards_NextCardId",
                        column: x => x.NextCardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cards_Cards_PreviousCardId",
                        column: x => x.PreviousCardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cards_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardSequences",
                columns: table => new
                {
                    CardSequenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardSequenceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardSequenceDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartCardId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardSequences", x => x.CardSequenceId);
                    table.ForeignKey(
                        name: "FK_CardSequences_Cards_StartCardId",
                        column: x => x.StartCardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardSequences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "DocFolders",
                columns: table => new
                {
                    DocFolderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContainerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlobName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocFolders", x => x.DocFolderId);
                    table.ForeignKey(
                        name: "FK_DocFolders_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImageFolders",
                columns: table => new
                {
                    ImageFolderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContainerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlobName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageFolders", x => x.ImageFolderId);
                    table.ForeignKey(
                        name: "FK_ImageFolders_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LinkFolders",
                columns: table => new
                {
                    LinkFolderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContainerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlobName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkFolders", x => x.LinkFolderId);
                    table.ForeignKey(
                        name: "FK_LinkFolders_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShareHashes",
                columns: table => new
                {
                    ShareHashId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardId = table.Column<int>(type: "int", nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShareHashes", x => x.ShareHashId);
                    table.ForeignKey(
                        name: "FK_ShareHashes_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardGroups_UserId",
                table: "CardGroups",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardGroupId",
                table: "Cards",
                column: "CardGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardSequenceId",
                table: "Cards",
                column: "CardSequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_NextCardId",
                table: "Cards",
                column: "NextCardId",
                unique: true,
                filter: "[NextCardId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_PreviousCardId",
                table: "Cards",
                column: "PreviousCardId",
                unique: true,
                filter: "[PreviousCardId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_UserId",
                table: "Cards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CardSequences_StartCardId",
                table: "CardSequences",
                column: "StartCardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardSequences_UserId",
                table: "CardSequences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocFolders_CardId",
                table: "DocFolders",
                column: "CardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_FUserId",
                table: "Friendships",
                column: "FUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_SUserId",
                table: "Friendships",
                column: "SUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageFolders_CardId",
                table: "ImageFolders",
                column: "CardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LinkFolders_CardId",
                table: "LinkFolders",
                column: "CardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShareHashes_CardId",
                table: "ShareHashes",
                column: "CardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_AdminId",
                table: "UserGroups",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUserGroup_UsersUserId",
                table: "UserUserGroup",
                column: "UsersUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_CardSequences_CardSequenceId",
                table: "Cards",
                column: "CardSequenceId",
                principalTable: "CardSequences",
                principalColumn: "CardSequenceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardGroups_Users_UserId",
                table: "CardGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Users_UserId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_CardSequences_Users_UserId",
                table: "CardSequences");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_CardGroups_CardGroupId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_CardSequences_CardSequenceId",
                table: "Cards");

            migrationBuilder.DropTable(
                name: "DocFolders");

            migrationBuilder.DropTable(
                name: "Friendships");

            migrationBuilder.DropTable(
                name: "ImageFolders");

            migrationBuilder.DropTable(
                name: "LinkFolders");

            migrationBuilder.DropTable(
                name: "ShareHashes");

            migrationBuilder.DropTable(
                name: "UserUserGroup");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "CardGroups");

            migrationBuilder.DropTable(
                name: "CardSequences");

            migrationBuilder.DropTable(
                name: "Cards");
        }
    }
}
