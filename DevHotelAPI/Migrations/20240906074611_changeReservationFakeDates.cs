using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHotelAPI.Migrations
{
    /// <inheritdoc />
    public partial class changeReservationFakeDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "From", "RoomNumber", "To" },
                values: new object[] { new DateTime(2027, 1, 16, 15, 15, 0, 0, DateTimeKind.Unspecified), 100, new DateTime(2027, 1, 18, 15, 15, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111112"),
                columns: new[] { "ClientId", "From", "RoomNumber", "To" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2027, 1, 16, 15, 15, 0, 0, DateTimeKind.Unspecified), 101, new DateTime(2027, 1, 18, 15, 15, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111113"),
                columns: new[] { "ClientId", "From", "RoomNumber", "To" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222221"), new DateTime(2027, 1, 16, 15, 15, 0, 0, DateTimeKind.Unspecified), 102, new DateTime(2027, 1, 18, 15, 15, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111114"),
                columns: new[] { "From", "RoomNumber", "To" },
                values: new object[] { new DateTime(2027, 1, 16, 15, 15, 0, 0, DateTimeKind.Unspecified), 103, new DateTime(2027, 1, 18, 15, 15, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111115"),
                columns: new[] { "From", "RoomNumber", "To" },
                values: new object[] { new DateTime(2027, 1, 16, 15, 15, 0, 0, DateTimeKind.Unspecified), 104, new DateTime(2027, 1, 18, 15, 15, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "From", "RoomNumber", "To" },
                values: new object[] { new DateTime(2024, 7, 7, 13, 28, 18, 523, DateTimeKind.Unspecified).AddTicks(1921), 102, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111112"),
                columns: new[] { "ClientId", "From", "RoomNumber", "To" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222221"), new DateTime(2024, 3, 19, 9, 22, 56, 353, DateTimeKind.Unspecified).AddTicks(2704), 108, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111113"),
                columns: new[] { "ClientId", "From", "RoomNumber", "To" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2024, 11, 30, 5, 17, 34, 183, DateTimeKind.Unspecified).AddTicks(3487), 103, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111114"),
                columns: new[] { "From", "RoomNumber", "To" },
                values: new object[] { new DateTime(2024, 8, 12, 1, 12, 12, 13, DateTimeKind.Unspecified).AddTicks(4270), 108, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111115"),
                columns: new[] { "From", "RoomNumber", "To" },
                values: new object[] { new DateTime(2024, 4, 23, 21, 6, 49, 843, DateTimeKind.Unspecified).AddTicks(5053), 103, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }
    }
}
