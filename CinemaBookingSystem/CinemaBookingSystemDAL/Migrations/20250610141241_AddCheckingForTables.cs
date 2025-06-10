using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaBookingSystemDAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckingForTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Session_Price",
                table: "Sessions",
                sql: "\"Price\" >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Seat_RowNumber",
                table: "Seats",
                sql: "\"RowNumber\" >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Seat_SeatNumber",
                table: "Seats",
                sql: "\"SeatNumber\" >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movie_Duration",
                table: "Movies",
                sql: "\"Duration\" >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movie_Rating",
                table: "Movies",
                sql: "\"Rating\" >= 2.0 AND \"Rating\" <= 10.0 ");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Hall_RowsAmount",
                table: "Halls",
                sql: "\"RowsAmount\" >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Hall_SeatsPerRow",
                table: "Halls",
                sql: "\"SeatsPerRow\" >= 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Session_Price",
                table: "Sessions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Seat_RowNumber",
                table: "Seats");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Seat_SeatNumber",
                table: "Seats");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Movie_Duration",
                table: "Movies");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Movie_Rating",
                table: "Movies");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Hall_RowsAmount",
                table: "Halls");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Hall_SeatsPerRow",
                table: "Halls");
        }
    }
}
