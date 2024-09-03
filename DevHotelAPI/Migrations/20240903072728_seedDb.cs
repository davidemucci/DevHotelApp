using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DevHotelAPI.Migrations
{
    /// <inheritdoc />
    public partial class seedDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rooms_RoomTypeId",
                table: "Rooms");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "RoomTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Address", "Email", "Name", "Password", "Surname" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222221"), "76098 Torphy Overpass", "Bernita_Konopelski43@gmail.com", null, "Ay7ZbbzDk0", null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "699 Walter Corners", "Guillermo.Cummerata30@hotmail.com", null, "lJ5hPDb6ee", null }
                });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "Id", "Description", "TotalNumber" },
                values: new object[,]
                {
                    { 1, "Room", 13 },
                    { 2, "TwinRoom", 39 },
                    { 3, "Triple", 15 },
                    { 4, "Suite", 41 }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "Id", "ClientId", "From", "RoomNumber", "To" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("22222222-2222-2222-2222-222222222221"), new DateTime(2024, 7, 7, 13, 28, 18, 523, DateTimeKind.Unspecified).AddTicks(1921), 102, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111112"), new Guid("22222222-2222-2222-2222-222222222221"), new DateTime(2024, 3, 19, 9, 22, 56, 353, DateTimeKind.Unspecified).AddTicks(2704), 108, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111113"), new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2024, 11, 30, 5, 17, 34, 183, DateTimeKind.Unspecified).AddTicks(3487), 103, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111114"), new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2024, 8, 12, 1, 12, 12, 13, DateTimeKind.Unspecified).AddTicks(4270), 108, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-1111-1111-1111-111111111115"), new Guid("22222222-2222-2222-2222-222222222221"), new DateTime(2024, 4, 23, 21, 6, 49, 843, DateTimeKind.Unspecified).AddTicks(5053), 103, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Number", "Description", "RoomTypeId" },
                values: new object[,]
                {
                    { 100, null, 1 },
                    { 101, null, 4 },
                    { 102, null, 2 },
                    { 103, null, 4 },
                    { 104, null, 2 },
                    { 105, null, 4 },
                    { 106, null, 2 },
                    { 107, null, 4 },
                    { 108, null, 2 },
                    { 109, null, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomTypeId",
                table: "Rooms",
                column: "RoomTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rooms_RoomTypeId",
                table: "Rooms");

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111112"));

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111113"));

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111114"));

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111115"));

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Number",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Number",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Number",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Number",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Number",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Number",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Number",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Number",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Number",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Number",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222221"));

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "RoomTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomTypeId",
                table: "Rooms",
                column: "RoomTypeId",
                unique: true);
        }
    }
}
