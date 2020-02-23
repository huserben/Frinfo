using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Frinfo.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Households",
                columns: table => new
                {
                    HouseholdId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    HouseholdCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Households", x => x.HouseholdId);
                });

            migrationBuilder.CreateTable(
                name: "Fridge",
                columns: table => new
                {
                    FridgeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HouseholdId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fridge", x => x.FridgeId);
                    table.ForeignKey(
                        name: "FK_Fridge_Households_HouseholdId",
                        column: x => x.HouseholdId,
                        principalTable: "Households",
                        principalColumn: "HouseholdId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FridgeItem",
                columns: table => new
                {
                    FridgeItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FridgeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: true),
                    ItemImage = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FridgeItem", x => x.FridgeItemId);
                    table.ForeignKey(
                        name: "FK_FridgeItem_Fridge_FridgeId",
                        column: x => x.FridgeId,
                        principalTable: "Fridge",
                        principalColumn: "FridgeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Households",
                columns: new[] { "HouseholdId", "HouseholdCode", "Name" },
                values: new object[] { 1, "1337", "Test Household" });

            migrationBuilder.InsertData(
                table: "Fridge",
                columns: new[] { "FridgeId", "HouseholdId", "Name" },
                values: new object[] { 1, 1, "Kitchen" });

            migrationBuilder.InsertData(
                table: "Fridge",
                columns: new[] { "FridgeId", "HouseholdId", "Name" },
                values: new object[] { 2, 1, "Cellar" });

            migrationBuilder.InsertData(
                table: "FridgeItem",
                columns: new[] { "FridgeItemId", "ExpirationDate", "FridgeId", "ItemImage", "Name" },
                values: new object[,]
                {
                    { 3, new DateTime(2020, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, "Salmon" },
                    { 4, null, 1, null, "Ice Cream" },
                    { 1, new DateTime(2022, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, null, "French Fries" },
                    { 2, null, 2, null, "Chicken Nuggets" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fridge_HouseholdId",
                table: "Fridge",
                column: "HouseholdId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgeItem_FridgeId",
                table: "FridgeItem",
                column: "FridgeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FridgeItem");

            migrationBuilder.DropTable(
                name: "Fridge");

            migrationBuilder.DropTable(
                name: "Households");
        }
    }
}
