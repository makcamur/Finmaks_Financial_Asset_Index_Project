using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finmaks_Financial_Asset_Index_Project.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exchanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseCurrencyCode = table.Column<int>(type: "int", nullable: false),
                    ForeignCurrencyCode = table.Column<int>(type: "int", nullable: false),
                    CashChangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CashExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CentralBankChangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CentralBankExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CrossRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exchanges", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exchanges");
        }
    }
}
