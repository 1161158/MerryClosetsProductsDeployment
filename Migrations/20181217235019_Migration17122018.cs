using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MerryClosets.Migrations
{
    public partial class Migration17122018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Finish_Price_PriceId",
                table: "Finish");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialAlgorithm_MaterialProduct_MaterialProductId",
                table: "MaterialAlgorithm");

            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Price_PriceId",
                table: "Materials");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCollection_ProductCollectionCatalog_ProductCollectionCatalogId",
                table: "ProductCollection");

            migrationBuilder.DropTable(
                name: "MaterialProduct");

            migrationBuilder.DropTable(
                name: "ProductCollectionCatalog");

            migrationBuilder.DropIndex(
                name: "IX_ProductCollection_ProductCollectionCatalogId",
                table: "ProductCollection");

            migrationBuilder.DropIndex(
                name: "IX_Materials_PriceId",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Finish_PriceId",
                table: "Finish");

            migrationBuilder.DropColumn(
                name: "ProductCollectionCatalogId",
                table: "ProductCollection");

            migrationBuilder.DropColumn(
                name: "PriceId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "PriceId",
                table: "Finish");

            migrationBuilder.RenameColumn(
                name: "MaterialProductId",
                table: "MaterialAlgorithm",
                newName: "ProductMaterialId");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialAlgorithm_MaterialProductId",
                table: "MaterialAlgorithm",
                newName: "IX_MaterialAlgorithm_ProductMaterialId");

            migrationBuilder.RenameColumn(
                name: "Reference",
                table: "Color",
                newName: "HexCode");

            migrationBuilder.AddColumn<int>(
                name: "RecSize",
                table: "SlotDefinition",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "ModelGroupId",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Max",
                table: "PartAlgorithm",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Min",
                table: "PartAlgorithm",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SizeType",
                table: "PartAlgorithm",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Finish",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "Finish",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Color",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExternal",
                table: "Categories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Animation",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    type = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogProductCollection",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CatalogReference = table.Column<string>(nullable: true),
                    ProductCollectionId = table.Column<long>(nullable: true),
                    CatalogId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogProductCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogProductCollection_Catalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "Catalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CatalogProductCollection_ProductCollection_ProductCollectionId",
                        column: x => x.ProductCollectionId,
                        principalTable: "ProductCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModelGroup",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RelativeURL = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    PriceId = table.Column<long>(nullable: true),
                    FinishId = table.Column<long>(nullable: true),
                    MaterialId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceHistory_Finish_FinishId",
                        column: x => x.FinishId,
                        principalTable: "Finish",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PriceHistory_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PriceHistory_Price_PriceId",
                        column: x => x.PriceId,
                        principalTable: "Price",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductMaterial",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductReference = table.Column<string>(nullable: true),
                    MaterialReference = table.Column<string>(nullable: true),
                    ProductId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMaterial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductMaterial_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Component",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    AnimationId = table.Column<long>(nullable: true),
                    ModelGroupId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Component", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Component_Animation_AnimationId",
                        column: x => x.AnimationId,
                        principalTable: "Animation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Component_ModelGroup_ModelGroupId",
                        column: x => x.ModelGroupId,
                        principalTable: "ModelGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ModelGroupId",
                table: "Products",
                column: "ModelGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogProductCollection_CatalogId",
                table: "CatalogProductCollection",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogProductCollection_ProductCollectionId",
                table: "CatalogProductCollection",
                column: "ProductCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Component_AnimationId",
                table: "Component",
                column: "AnimationId");

            migrationBuilder.CreateIndex(
                name: "IX_Component_ModelGroupId",
                table: "Component",
                column: "ModelGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistory_FinishId",
                table: "PriceHistory",
                column: "FinishId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistory_MaterialId",
                table: "PriceHistory",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistory_PriceId",
                table: "PriceHistory",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMaterial_ProductId",
                table: "ProductMaterial",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialAlgorithm_ProductMaterial_ProductMaterialId",
                table: "MaterialAlgorithm",
                column: "ProductMaterialId",
                principalTable: "ProductMaterial",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ModelGroup_ModelGroupId",
                table: "Products",
                column: "ModelGroupId",
                principalTable: "ModelGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialAlgorithm_ProductMaterial_ProductMaterialId",
                table: "MaterialAlgorithm");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ModelGroup_ModelGroupId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "CatalogProductCollection");

            migrationBuilder.DropTable(
                name: "Component");

            migrationBuilder.DropTable(
                name: "PriceHistory");

            migrationBuilder.DropTable(
                name: "ProductMaterial");

            migrationBuilder.DropTable(
                name: "Animation");

            migrationBuilder.DropTable(
                name: "ModelGroup");

            migrationBuilder.DropIndex(
                name: "IX_Products_ModelGroupId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RecSize",
                table: "SlotDefinition");

            migrationBuilder.DropColumn(
                name: "ModelGroupId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Max",
                table: "PartAlgorithm");

            migrationBuilder.DropColumn(
                name: "Min",
                table: "PartAlgorithm");

            migrationBuilder.DropColumn(
                name: "SizeType",
                table: "PartAlgorithm");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Finish");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Finish");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Color");

            migrationBuilder.DropColumn(
                name: "IsExternal",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "ProductMaterialId",
                table: "MaterialAlgorithm",
                newName: "MaterialProductId");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialAlgorithm_ProductMaterialId",
                table: "MaterialAlgorithm",
                newName: "IX_MaterialAlgorithm_MaterialProductId");

            migrationBuilder.RenameColumn(
                name: "HexCode",
                table: "Color",
                newName: "Reference");

            migrationBuilder.AddColumn<long>(
                name: "ProductCollectionCatalogId",
                table: "ProductCollection",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PriceId",
                table: "Materials",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PriceId",
                table: "Finish",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MaterialProduct",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MaterialReference = table.Column<string>(nullable: true),
                    ProductId = table.Column<long>(nullable: true),
                    ProductReference = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialProduct_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductCollectionCatalog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CatalogId = table.Column<long>(nullable: true),
                    CatalogReference = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCollectionCatalog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCollectionCatalog_Catalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "Catalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCollection_ProductCollectionCatalogId",
                table: "ProductCollection",
                column: "ProductCollectionCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_PriceId",
                table: "Materials",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_Finish_PriceId",
                table: "Finish",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialProduct_ProductId",
                table: "MaterialProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCollectionCatalog_CatalogId",
                table: "ProductCollectionCatalog",
                column: "CatalogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Finish_Price_PriceId",
                table: "Finish",
                column: "PriceId",
                principalTable: "Price",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialAlgorithm_MaterialProduct_MaterialProductId",
                table: "MaterialAlgorithm",
                column: "MaterialProductId",
                principalTable: "MaterialProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Price_PriceId",
                table: "Materials",
                column: "PriceId",
                principalTable: "Price",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCollection_ProductCollectionCatalog_ProductCollectionCatalogId",
                table: "ProductCollection",
                column: "ProductCollectionCatalogId",
                principalTable: "ProductCollectionCatalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
