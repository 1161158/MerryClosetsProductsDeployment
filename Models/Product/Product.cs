using System;
using System.Collections.Generic;
using MerryClosets.Models.DTO;
using MerryClosets.Models.Material;

namespace MerryClosets.Models.Product
{
    public class Product : BaseEntity
    {
        /**
         * Name of the product.
         */
        public string Name { get; set; }

        /**
         * Description of the product.
         */
        public string Description { get; set; }

        /**
         * Reference of the category to which the product belongs.
         */
        public string CategoryReference { get; set; }

        /**
         * Base price of the product.
         */
        public Price Price { get; set; }

        /**
         * Other products which the product can be built with.
         */
        public List<Part> Parts { get; set; } = new List<Part>();

        /**
         * Materials in which the product can be built.
         */
        public List<ProductMaterial> ProductMaterialList { get; set; } = new List<ProductMaterial>();

        /**
         * All possible dimensions that the product can be configured in.
         */
        public List<DimensionValues> Dimensions { get; set; } = new List<DimensionValues>();

        /**
         * Definitions that the slots this product may have, are required to follow.
         */
        public SlotDefinition SlotDefinition { get; set; }

        public Product(string reference, string name, string description, string categoryReference, Price price,
            List<Part> parts, List<ProductMaterial> productMaterialList, List<DimensionValues> dimensions,
            SlotDefinition slotDef)
        {
            this.Reference = reference;
            this.Name = name;
            this.Description = description;
            this.CategoryReference = categoryReference;
            this.Price = price;
            this.Parts = parts;
            this.ProductMaterialList = productMaterialList;
            this.Dimensions = dimensions;
            this.SlotDefinition = slotDef;
        }

        public Product(string reference, string name, string description, string categoryReference, Price price,
            SlotDefinition slotDef)
        {
            this.Reference = reference;
            this.Name = name;
            this.Description = description;
            this.CategoryReference = categoryReference;
            this.Price = price;
            this.SlotDefinition = slotDef;
        }

        protected Product()
        {
        }

        /**
         * Method will verify if the material with the passed reference is associated with this product.
         */
        public bool IsAssociatedWithMaterial(string materialReference)
        {
            return this.ProductMaterialList.Contains(new ProductMaterial(this.Reference, materialReference));
        }

        /**
         * Method will verify if the product with the passed reference is associated with this product.
         */
        public bool IsAssociatedWithProduct(string productReference)
        {
            return this.Parts.Contains(new Part(productReference));
        }

        /**
         * Method will verify if the passed dimension values is associated with this product.
         */
        public bool IsAssociatedWithDimensionValues(DimensionValues dimensionValuesToCheck)
        {
            return this.Dimensions.Contains(dimensionValuesToCheck);
        }

        public void AddProductMaterial(ProductMaterial materialProd)
        {
            ProductMaterialList.Add(materialProd);
        }

        public void AddPart(Part part)
        {
            Parts.Add(part);
        }

        public void AddDimensionValues(DimensionValues dimValues)
        {
            Dimensions.Add(dimValues);
        }

        public void RemoveMaterialAndRespectiveAlgorithms(Material.Material material)
        {
            ProductMaterialList.Remove(new ProductMaterial(this.Reference, material.Reference));
        }

        public void RemoveProductAndRespectiveAlgorithms(Product product)
        {
            Parts.Remove(new Part(product.Reference));
        }

        public void RemoveDimensionValues(DimensionValues dimValues)
        {
            Dimensions.Remove(dimValues);
        }

        public bool ChosenDimensionDtoIsValid(ConfiguredDimensionDto configured)
        {
            foreach (var dimensionValue in this.Dimensions)
            {
                if (dimensionValue.CheckIfItBelongs(configured.Width, configured.Height, configured.Depth))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            var p = obj as Product;
            return string.Equals(this.Reference, p.Reference, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return System.Tuple.Create(Reference).GetHashCode();
        }

        public bool ConfiguredMaterialDtoExists(ConfiguredMaterialDto configuredMaterial)
        {
            foreach (var material in ProductMaterialList)
            {
                if (string.Equals(material.MaterialReference, configuredMaterial.OriginMaterialReference,
                    StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public Part IsProductPart(Product product)
        {
            foreach (var part in this.Parts)
            {
                if (string.Equals(part.ProductReference, product.Reference, StringComparison.Ordinal))
                {
                    return part;
                }
            }

            return null;
        }
    }
}