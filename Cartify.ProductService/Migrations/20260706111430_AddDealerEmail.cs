using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cartify.ProductService.Migrations
{
    /// <inheritdoc />
    public partial class AddDealerEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DealerEmail",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DealerEmail",
                table: "Products");
        }
    }
}
