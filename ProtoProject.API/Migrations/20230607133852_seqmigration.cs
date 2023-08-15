using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoProject.API.Migrations
{
    /// <inheritdoc />
    public partial class seqmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardSequences_Cards_StartCardId",
                table: "CardSequences");

            migrationBuilder.DropIndex(
                name: "IX_CardSequences_StartCardId",
                table: "CardSequences");

            migrationBuilder.AddColumn<int>(
                name: "StartCardCardId",
                table: "CardSequences",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardSequences_StartCardCardId",
                table: "CardSequences",
                column: "StartCardCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardSequences_Cards_StartCardCardId",
                table: "CardSequences",
                column: "StartCardCardId",
                principalTable: "Cards",
                principalColumn: "CardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardSequences_Cards_StartCardCardId",
                table: "CardSequences");

            migrationBuilder.DropIndex(
                name: "IX_CardSequences_StartCardCardId",
                table: "CardSequences");

            migrationBuilder.DropColumn(
                name: "StartCardCardId",
                table: "CardSequences");

            migrationBuilder.CreateIndex(
                name: "IX_CardSequences_StartCardId",
                table: "CardSequences",
                column: "StartCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardSequences_Cards_StartCardId",
                table: "CardSequences",
                column: "StartCardId",
                principalTable: "Cards",
                principalColumn: "CardId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
