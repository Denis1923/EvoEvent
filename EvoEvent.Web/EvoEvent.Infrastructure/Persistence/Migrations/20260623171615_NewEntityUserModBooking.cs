using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvoEvent.Web.Migrations
{
    /// <inheritdoc />
    public partial class NewEntityUserModBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Сначала создаем таблицу users
			migrationBuilder.CreateTable(
				name: "users",
				schema: "catalog",
				columns: table => new
				{
					UserId = table.Column<Guid>(type: "uuid", nullable: false),
					Login = table.Column<string>(type: "text", nullable: false),
					HashPassword = table.Column<string>(type: "text", nullable: false),
					Role = table.Column<int>(type: "integer", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_users", x => x.UserId);
				});

			// Добавляем столбец UserId в таблицу bookings (ДО создания индекса)
			migrationBuilder.AddColumn<Guid>(
				name: "UserId",
				schema: "catalog",
				table: "bookings",
				type: "uuid",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			// Теперь создаем индекс (столбец уже существует)
			migrationBuilder.CreateIndex(
				name: "IX_bookings_UserId",
				schema: "catalog",
				table: "bookings",
				column: "UserId");

			// Создаем индекс для Login
			migrationBuilder.CreateIndex(
				name: "IX_users_Login",
				schema: "catalog",
				table: "users",
				column: "Login",
				unique: true);

			// Добавляем внешний ключ
			migrationBuilder.AddForeignKey(
				name: "FK_bookings_users_UserId",
				schema: "catalog",
				table: "bookings",
				column: "UserId",
				principalSchema: "catalog",
				principalTable: "users",
				principalColumn: "UserId",
				onDelete: ReferentialAction.Cascade);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookings_users_UserId",
                schema: "catalog",
                table: "bookings");

            migrationBuilder.DropTable(
                name: "users",
                schema: "catalog");

            migrationBuilder.DropIndex(
                name: "IX_bookings_UserId",
                schema: "catalog",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "catalog",
                table: "bookings");
        }
    }
}
