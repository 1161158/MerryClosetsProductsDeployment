using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MerryClosets.Migrations
{
    public partial class _20122018Migrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Catalogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Reference = table.Column<string>(nullable: true),
                    Version = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Reference = table.Column<string>(nullable: true),
                    Version = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ParentCategoryReference = table.Column<string>(nullable: true),
                    IsExternal = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Reference = table.Column<string>(nullable: true),
                    Version = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguredDimension",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Height = table.Column<int>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    Depth = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguredDimension", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguredMaterial",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OriginMaterialReference = table.Column<string>(nullable: true),
                    ColorReference = table.Column<string>(nullable: true),
                    FinishReference = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguredMaterial", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Reference = table.Column<string>(nullable: true),
                    Version = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
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
                name: "Price",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Price", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SlotDefinition",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MinSize = table.Column<int>(nullable: false),
                    MaxSize = table.Column<int>(nullable: false),
                    RecSize = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotDefinition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCollection",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CollectionReference = table.Column<string>(nullable: true),
                    ConfiguredProductReference = table.Column<string>(nullable: true),
                    CollectionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCollection_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Color",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    HexCode = table.Column<string>(nullable: true),
                    MaterialId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Color", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Color_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Finish",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Reference = table.Column<string>(nullable: true),
                    Version = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    MaterialId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Finish", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Finish_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
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

            migrationBuilder.CreateTable(
                name: "ConfiguredProducts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Reference = table.Column<string>(nullable: true),
                    Version = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    ProductReference = table.Column<string>(nullable: true),
                    ConfiguredMaterialId = table.Column<long>(nullable: true),
                    ConfiguredDimensionId = table.Column<long>(nullable: true),
                    PriceId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguredProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguredProducts_ConfiguredDimension_ConfiguredDimensionId",
                        column: x => x.ConfiguredDimensionId,
                        principalTable: "ConfiguredDimension",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConfiguredProducts_ConfiguredMaterial_ConfiguredMaterialId",
                        column: x => x.ConfiguredMaterialId,
                        principalTable: "ConfiguredMaterial",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConfiguredProducts_Price_PriceId",
                        column: x => x.PriceId,
                        principalTable: "Price",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Reference = table.Column<string>(nullable: true),
                    Version = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CategoryReference = table.Column<string>(nullable: true),
                    PriceId = table.Column<long>(nullable: true),
                    SlotDefinitionId = table.Column<long>(nullable: true),
                    ModelGroupId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_ModelGroup_ModelGroupId",
                        column: x => x.ModelGroupId,
                        principalTable: "ModelGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Price_PriceId",
                        column: x => x.PriceId,
                        principalTable: "Price",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_SlotDefinition_SlotDefinitionId",
                        column: x => x.SlotDefinitionId,
                        principalTable: "SlotDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "ConfiguredPart",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConfiguredChildReference = table.Column<string>(nullable: true),
                    ChosenSlotReference = table.Column<string>(nullable: true),
                    ConfiguredProductId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguredPart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguredPart_ConfiguredProducts_ConfiguredProductId",
                        column: x => x.ConfiguredProductId,
                        principalTable: "ConfiguredProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguredSlot",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Size = table.Column<int>(nullable: false),
                    Reference = table.Column<string>(nullable: true),
                    ConfiguredProductId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguredSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguredSlot_ConfiguredProducts_ConfiguredProductId",
                        column: x => x.ConfiguredProductId,
                        principalTable: "ConfiguredProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DimensionValues",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimensionValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DimensionValues_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Part",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductReference = table.Column<string>(nullable: true),
                    ProductId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Part", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Part_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
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
                name: "DimensionAlgorithm",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DimensionValuesId = table.Column<long>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    FirstValueDesc = table.Column<string>(nullable: true),
                    SecondValueDesc = table.Column<string>(nullable: true),
                    Operator = table.Column<string>(nullable: true),
                    Ratio = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimensionAlgorithm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DimensionAlgorithm_DimensionValues_DimensionValuesId",
                        column: x => x.DimensionValuesId,
                        principalTable: "DimensionValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DimensionValuesId = table.Column<long>(nullable: true),
                    DimensionValuesId1 = table.Column<long>(nullable: true),
                    DimensionValuesId2 = table.Column<long>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    MinValue = table.Column<int>(nullable: true),
                    MaxValue = table.Column<int>(nullable: true),
                    Value = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Values_DimensionValues_DimensionValuesId",
                        column: x => x.DimensionValuesId,
                        principalTable: "DimensionValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Values_DimensionValues_DimensionValuesId1",
                        column: x => x.DimensionValuesId1,
                        principalTable: "DimensionValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Values_DimensionValues_DimensionValuesId2",
                        column: x => x.DimensionValuesId2,
                        principalTable: "DimensionValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PartAlgorithm",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Discriminator = table.Column<string>(nullable: false),
                    PartId = table.Column<long>(nullable: true),
                    SizeType = table.Column<string>(nullable: true),
                    Min = table.Column<int>(nullable: true),
                    Max = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartAlgorithm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartAlgorithm_Part_PartId",
                        column: x => x.PartId,
                        principalTable: "Part",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialAlgorithm",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductMaterialId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialAlgorithm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialAlgorithm_ProductMaterial_ProductMaterialId",
                        column: x => x.ProductMaterialId,
                        principalTable: "ProductMaterial",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogProductCollection_CatalogId",
                table: "CatalogProductCollection",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogProductCollection_ProductCollectionId",
                table: "CatalogProductCollection",
                column: "ProductCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Color_MaterialId",
                table: "Color",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Component_AnimationId",
                table: "Component",
                column: "AnimationId");

            migrationBuilder.CreateIndex(
                name: "IX_Component_ModelGroupId",
                table: "Component",
                column: "ModelGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguredPart_ConfiguredProductId",
                table: "ConfiguredPart",
                column: "ConfiguredProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguredProducts_ConfiguredDimensionId",
                table: "ConfiguredProducts",
                column: "ConfiguredDimensionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguredProducts_ConfiguredMaterialId",
                table: "ConfiguredProducts",
                column: "ConfiguredMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguredProducts_PriceId",
                table: "ConfiguredProducts",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguredSlot_ConfiguredProductId",
                table: "ConfiguredSlot",
                column: "ConfiguredProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionAlgorithm_DimensionValuesId",
                table: "DimensionAlgorithm",
                column: "DimensionValuesId");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionValues_ProductId",
                table: "DimensionValues",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Finish_MaterialId",
                table: "Finish",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialAlgorithm_ProductMaterialId",
                table: "MaterialAlgorithm",
                column: "ProductMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Part_ProductId",
                table: "Part",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PartAlgorithm_PartId",
                table: "PartAlgorithm",
                column: "PartId");

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
                name: "IX_ProductCollection_CollectionId",
                table: "ProductCollection",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMaterial_ProductId",
                table: "ProductMaterial",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ModelGroupId",
                table: "Products",
                column: "ModelGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_PriceId",
                table: "Products",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SlotDefinitionId",
                table: "Products",
                column: "SlotDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Values_DimensionValuesId",
                table: "Values",
                column: "DimensionValuesId");

            migrationBuilder.CreateIndex(
                name: "IX_Values_DimensionValuesId1",
                table: "Values",
                column: "DimensionValuesId1");

            migrationBuilder.CreateIndex(
                name: "IX_Values_DimensionValuesId2",
                table: "Values",
                column: "DimensionValuesId2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogProductCollection");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Color");

            migrationBuilder.DropTable(
                name: "Component");

            migrationBuilder.DropTable(
                name: "ConfiguredPart");

            migrationBuilder.DropTable(
                name: "ConfiguredSlot");

            migrationBuilder.DropTable(
                name: "DimensionAlgorithm");

            migrationBuilder.DropTable(
                name: "MaterialAlgorithm");

            migrationBuilder.DropTable(
                name: "PartAlgorithm");

            migrationBuilder.DropTable(
                name: "PriceHistory");

            migrationBuilder.DropTable(
                name: "Values");

            migrationBuilder.DropTable(
                name: "Catalogs");

            migrationBuilder.DropTable(
                name: "ProductCollection");

            migrationBuilder.DropTable(
                name: "Animation");

            migrationBuilder.DropTable(
                name: "ConfiguredProducts");

            migrationBuilder.DropTable(
                name: "ProductMaterial");

            migrationBuilder.DropTable(
                name: "Part");

            migrationBuilder.DropTable(
                name: "Finish");

            migrationBuilder.DropTable(
                name: "DimensionValues");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "ConfiguredDimension");

            migrationBuilder.DropTable(
                name: "ConfiguredMaterial");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ModelGroup");

            migrationBuilder.DropTable(
                name: "Price");

            migrationBuilder.DropTable(
                name: "SlotDefinition");
        }
    }
}
