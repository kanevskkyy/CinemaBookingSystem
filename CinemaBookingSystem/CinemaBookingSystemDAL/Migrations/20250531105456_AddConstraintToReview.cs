using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaBookingSystemDAL.Migrations
{
    /// <inheritdoc />
    public partial class AddConstraintToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Review_Rating_Range",
                table: "Reviews",
                sql: "\"Rating\" >= 1 AND \"Rating\" <= 5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Review_Rating_Range",
                table: "Reviews");
        }
    }
}
