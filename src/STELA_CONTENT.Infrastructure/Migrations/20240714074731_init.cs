using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STELA_CONTENT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdditionalServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ColorName = table.Column<string>(type: "text", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: true),
                    Hex = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Memorials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    StelaLength = table.Column<float>(type: "real", nullable: false),
                    StelaWidth = table.Column<float>(type: "real", nullable: false),
                    StelaHeight = table.Column<float>(type: "real", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memorials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioMemorials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CemeteryName = table.Column<string>(type: "text", nullable: false),
                    Images = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioMemorials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemorialMaterials",
                columns: table => new
                {
                    MemorialId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemorialMaterials", x => new { x.MemorialId, x.MaterialId });
                    table.ForeignKey(
                        name: "FK_MemorialMaterials_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemorialMaterials_Memorials_MemorialId",
                        column: x => x.MemorialId,
                        principalTable: "Memorials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioMemorialMaterials",
                columns: table => new
                {
                    PortfolioMemorialId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemorialMaterialId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioMemorialMaterials", x => new { x.MemorialMaterialId, x.PortfolioMemorialId });
                    table.ForeignKey(
                        name: "FK_PortfolioMemorialMaterials_Materials_MemorialMaterialId",
                        column: x => x.MemorialMaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PortfolioMemorialMaterials_PortfolioMemorials_PortfolioMemo~",
                        column: x => x.PortfolioMemorialId,
                        principalTable: "PortfolioMemorials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalServices_Name",
                table: "AdditionalServices",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materials_Name",
                table: "Materials",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemorialMaterials_MaterialId",
                table: "MemorialMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioMemorialMaterials_PortfolioMemorialId",
                table: "PortfolioMemorialMaterials",
                column: "PortfolioMemorialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdditionalServices");

            migrationBuilder.DropTable(
                name: "MemorialMaterials");

            migrationBuilder.DropTable(
                name: "PortfolioMemorialMaterials");

            migrationBuilder.DropTable(
                name: "Memorials");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "PortfolioMemorials");
        }
    }
}
