using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cartify.OrderService.Migrations
{
    /// <inheritdoc />
    public partial class AddFulfillment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DealerEmail",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryPartnerEmail",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DealerEmail",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryPartnerEmail",
                table: "Orders");
        }
    }
}
