using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class SubCategoryDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SubCategoryId",
                table: "Tickets",
                column: "SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_TicketSubCategory_SubCategoryId",
                table: "Tickets",
                column: "SubCategoryId",
                principalTable: "TicketSubCategory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_TicketSubCategory_SubCategoryId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_SubCategoryId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "Tickets");
        }
    }
}
