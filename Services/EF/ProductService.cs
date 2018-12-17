using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MerryClosets.Models.Animation;
using MerryClosets.Models.Category;
using MerryClosets.Models.DTO;
using MerryClosets.Models.DTO.DTOValidators;
using MerryClosets.Models.Material;
using MerryClosets.Models.Product;
using MerryClosets.Models.Restriction;
using MerryClosets.Repositories.Interfaces;
using MerryClosets.Services.Interfaces;
using MerryClosets.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace MerryClosets.Services.EF
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;

        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMaterialRepository _materialRepository;

        private readonly ProductDTOValidator _productDTOValidator;
        private readonly DimensionValuesDTOValidator _dimensionValuesDTOValidator;
        private readonly CategoryDTOValidator _categoryDTOValidator;
        private readonly MaterialDTOValidator _materialDTOValidator;
        private readonly AlgorithmDTOValidator _algorithmDTOValidator;
        private readonly ModelGroupDTOValidator _modelGroupDTOValidator;

        public ProductService(IMapper mapper, IProductRepository productRepository,
            ICategoryRepository categoryRepository, IMaterialRepository materialRepository,
            ProductDTOValidator productDTOValidator, CategoryDTOValidator categoryDTOValidator,
            MaterialDTOValidator materialDTOValidator, AlgorithmDTOValidator algorithmDTOValidator,
            DimensionValuesDTOValidator dimensionValuesDTOValidator, ModelGroupDTOValidator modelGroupDTOValidator)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _materialRepository = materialRepository;
            _productDTOValidator = productDTOValidator;
            _categoryDTOValidator = categoryDTOValidator;
            _materialDTOValidator = materialDTOValidator;
            _algorithmDTOValidator = algorithmDTOValidator;
            _dimensionValuesDTOValidator = dimensionValuesDTOValidator;
            _modelGroupDTOValidator = modelGroupDTOValidator;
        }

        /**
         * Private method used to verify the existence of a product in the DB, through its unique reference.
         */
        private bool ProductExists(string reference)
        {
            var product = _productRepository.GetByReference(reference);
            return product != null;
        }

        /**
         * Private method used to verify the existence of category in the DB, through its unique reference.
         */
        private bool CategoryExists(string reference)
        {
            var category = _categoryRepository.GetByReference(reference);
            return category != null;
        }

        /**
         * Private method used to verify the existence of material in the DB, through its unique reference.
         */
        private bool MaterialExists(string reference)
        {
            var material = _materialRepository.GetByReference(reference);
            return material != null;
        }

        private bool ExistsAndIsActive(string reference)
        {
            var product = _productRepository.GetByReference(reference);
            if (product != null && product.IsActive)
            {
                return true;
            }

            return false;
        }

        /**
         * Method that will verify materials and respective algorithms (in the form of ProductMaterial objects) that will be added to the product being created.
         * 
         * Validations performed:
         * 1. The received list has 1 or more elements;
         FOREACH RECEIVED ProductMaterial {
            * 2. Validation of the material's reference in current ProductMaterial (database);
                FOREACH RECEIVED MaterialAlgorithm {
                    * 3. Validation of current MaterialAlgorithm (business rules)
                    * 4. Validation for duplication between each MaterialAlgorithm received.
                }
            * 5. Validation for duplication between each ProductMaterial item received
         }
         */
        private ValidationOutput VerifyMaterialsAndRespectiveAlgorithms(ProductDto dto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            //1.
            if (dto.ProductMaterialList.Count == 0)
            {
                validationOutput.AddError("Materials", "No materials were selected!");
                return validationOutput;
            }

            List<ProductMaterial> productMaterialListToAdd = new List<ProductMaterial>();

            foreach (var productMaterialDto in dto.ProductMaterialList)
            {
                //2.
                validationOutput = new ValidationOutputNotFound();
                if (!MaterialExists(productMaterialDto.MaterialReference))
                {
                    validationOutput.AddError("Reference of material",
                        "No material with the reference '" + productMaterialDto.MaterialReference +
                        "' exists in the system.");
                    return validationOutput;
                }

                List<MaterialAlgorithmDto>
                    materialAlgorithmDtoListToAdd =
                        new List<MaterialAlgorithmDto>(); //Serves no purpose other than to assert if there are duplicates
                foreach (var materialAlgorithmDto in productMaterialDto.Algorithms)
                {
                    //3.
                    validationOutput = _algorithmDTOValidator.DTOIsValid(materialAlgorithmDto);
                    if (validationOutput.HasErrors())
                    {
                        return validationOutput;
                    }

                    //4.
                    validationOutput = new ValidationOutputBadRequest();
                    if (materialAlgorithmDtoListToAdd.Contains(materialAlgorithmDto))
                    {
                        validationOutput.AddError("Material algorithm",
                            "A received material algorithm between product'" + dto.Reference + "' and material '" +
                            productMaterialDto.MaterialReference +
                            "' is duplicated in the list of selected algorithms.");
                        return validationOutput;
                    }

                    materialAlgorithmDtoListToAdd.Add(materialAlgorithmDto);
                }

                ProductMaterial productMaterial = _mapper.Map<ProductMaterial>(productMaterialDto);

                validationOutput = new ValidationOutputBadRequest();
                //5.
                if (productMaterialListToAdd.Contains(productMaterial))
                {
                    validationOutput.AddError("Material",
                        "Material '" + productMaterialDto.MaterialReference +
                        "' and respective algorithms are duplicated in the list of materials (and chosen algorithms) selected!");
                    return validationOutput;
                }

                productMaterialDto.ProductReference = dto.Reference;
                productMaterialListToAdd.Add(productMaterial);
            }

            return validationOutput;
        }

        /**
         * Method that will verify the products and respective algorithms (in the form of Part objects) that will be added to the product being created.
         * 
         * Validations performed:
         * 1. The received list has 1 or more elements;
         FOREACH RECEIVED Part {
            * 2. Validation of the products's reference in each Part (database);
                FOREACH RECEIVED PartAlgorithm {
                    * 3. Validation of each PartAlgorithm (business rules)
                    * 4. Validation for duplication between each PartAlgorithm received
                }
            * 5. Validation for duplication between each Part object received
         }
         */
        private ValidationOutput VerifyProductsAndRespectiveAlgorithms(ProductDto dto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            //1.
            //            if (dto.Parts.Count == 0)
            //            {
            //                validationOutput.AddError("Products", "No products (or respective algorithms) were selected!");
            //                return validationOutput;
            //            }

            if (dto.Parts.Count > 0)
            {
                List<Part> partListToAdd = new List<Part>();

                foreach (var partDto in dto.Parts)
                {
                    //2.
                    validationOutput = new ValidationOutputNotFound();
                    if (!ProductExists(partDto.ProductReference))
                    {
                        validationOutput.AddError("Reference of product in a part",
                            "No product with the reference '" + partDto.ProductReference + "' exists in the system.");
                        return validationOutput;
                    }

                    List<PartAlgorithmDto>
                        partAlgorithmDtoListToAdd =
                            new List<PartAlgorithmDto>(); //Serves no purpose other than to assert if there are duplicates
                    foreach (var partAlgorithmDto in partDto.Algorithms)
                    {
                        //3.
                        validationOutput = _algorithmDTOValidator.DTOIsValid(partAlgorithmDto);
                        if (validationOutput.HasErrors())
                        {
                            return validationOutput;
                        }

                        //4.
                        validationOutput = new ValidationOutputBadRequest();
                        if (partAlgorithmDtoListToAdd.Contains(partAlgorithmDto))
                        {
                            validationOutput.AddError("Part algorithm",
                                "A received part algorithm between main product'" + dto.Reference + "' and product '" +
                                partDto.ProductReference + "' is duplicated.");
                            return validationOutput;
                        }

                        partAlgorithmDtoListToAdd.Add(partAlgorithmDto);
                    }

                    Part part = _mapper.Map<Part>(partDto);

                    validationOutput = new ValidationOutputBadRequest();
                    //5.
                    if (partListToAdd.Contains(part))
                    {
                        validationOutput.AddError("Product",
                            "Product '" + partDto.ProductReference + "' and respective algorithms are duplicated.");
                        return validationOutput;
                    }

                    partListToAdd.Add(part);
                }
            }

            return validationOutput;
        }

        /**
         * Method that will add new dimension values and respective algorithms (in the form of DimensionValues objects) to the product with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. The received list has 1 or more elements;
         FOREACH RECEIVED DimensionValues {
            * 2. Validation of dimensions in each DimensionValues (business rules);
                FOREACH RECEIVED DimensionAlgorithm {
                    * 3. Validation of each DimensionAlgorithm (business rules)
                    * 4. Validation for duplication between each DimensionAlgorithm received
                }
            * 5. Validation for duplication between each DimensionValues item received
         }
         */
        private ValidationOutput VerifyVariousDimensionValues(ProductDto dto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            //1.
            if (dto.Dimensions.Count == 0)
            {
                validationOutput.AddError("Dimension values", "No set of dimension values were defined!");
                return validationOutput;
            }

            List<DimensionValues> dimensionValuesListToAdd = new List<DimensionValues>();

            foreach (var dimensionValuesDto in dto.Dimensions)
            {
                //2.
                validationOutput = _dimensionValuesDTOValidator.DTOIsValid(dimensionValuesDto);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }

                DimensionValues dimensionValues = _mapper.Map<DimensionValues>(dimensionValuesDto);

                List<DimensionAlgorithmDto>
                    dimensionAlgorithmDtoList =
                        new List<DimensionAlgorithmDto>(); //Serves no purpose other than to assert if there are duplicates
                foreach (var dimensionAlgorithmDto in dimensionValuesDto.Algorithms)
                {
                    //3.
                    validationOutput = _algorithmDTOValidator.DTOIsValid(dimensionAlgorithmDto);
                    if (validationOutput.HasErrors())
                    {
                        return validationOutput;
                    }

                    //4.
                    validationOutput = new ValidationOutputBadRequest();
                    if (dimensionAlgorithmDtoList.Contains(dimensionAlgorithmDto))
                    {
                        validationOutput.AddError("Dimension algorithm",
                            "A dimension algorithm between a set of dimension values and the product'" + dto.Reference +
                            "' is duplicated.");
                        return validationOutput;
                    }

                    dimensionAlgorithmDtoList.Add(dimensionAlgorithmDto);
                }

                validationOutput = new ValidationOutputBadRequest();
                //5.
                if (dimensionValuesListToAdd.Contains(dimensionValues))
                {
                    validationOutput.AddError("Dimension values",
                        "A set of dimension values and respective algorithms are duplicated.");
                    return validationOutput;
                }

                dimensionValuesListToAdd.Add(dimensionValues);
            }

            return validationOutput;
        }

        // ============ Methods to CREATE something ============

        /**
         * Method that will validate and create a new product in the database.
         *
         * Validations performed:
         * 1. Validation of the new product's reference (business rules);
         * 2. Validation of the new product's reference (database);
         * 3. Validation of the received info. (name, description, price, category, slot settings, materials, parts, dimension values) (business rules)
         */
        public ValidationOutput Register(ProductDto dto)
        {
            //1.
            ValidationOutput validationOutput = _productDTOValidator.DTOReferenceIsValid(dto.Reference);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            //2.
            validationOutput = new ValidationOutputBadRequest();
            if (ProductExists(dto.Reference))
            {
                validationOutput.AddError("Reference of product",
                    "A product with the reference '" + dto.Reference + "' already exists in the system!");
                return validationOutput;
            }

            //3.
            validationOutput = _productDTOValidator.DTOIsValidForRegister(dto);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            validationOutput = new ValidationOutputNotFound();
            if (!string.IsNullOrEmpty(dto.CategoryReference) && !CategoryExists(dto.CategoryReference))
            {
                validationOutput.AddError("Reference of category",
                    "No category with the reference '" + dto.CategoryReference + "' exists in the system.");
                return validationOutput;
            }

            if (dto.ProductMaterialList.Count > 0)
            {
                validationOutput = VerifyMaterialsAndRespectiveAlgorithms(dto);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }
            }

            if (dto.Parts.Count > 0)
            {
                validationOutput = VerifyProductsAndRespectiveAlgorithms(dto);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }
            }

            if (dto.Dimensions.Count > 0)
            {
                validationOutput = VerifyVariousDimensionValues(dto);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }
            }

            //If we reached here then that means we can add the new product
            Product newProduct = _mapper.Map<Product>(dto);
            validationOutput.DesiredReturn = _mapper.Map<ProductDto>(_productRepository.Add(newProduct));

            return validationOutput;
        }


        // ============ Methods to GET something ============

        /**
         * Method that will return either the product in the form of a DTO that has the passed reference OR all the errors found when trying to do so.
         * 
         * Validations performed:
         * 1. Validation of the passed product's reference (database);
         * 
         * This method can return a soft-deleted product.
         */
        public ValidationOutput GetByReference(string reference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!ProductExists(reference))
            {
                validationOutput.AddError("Reference of product",
                    "No product with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            validationOutput.DesiredReturn = _mapper.Map<ProductDto>(_productRepository.GetByReference(reference));
            return validationOutput;
        }

        /**
         * Method that will return either the product in the form of a DTO that has the passed reference OR all the errors found when trying to do so.
         * 
         * Validations performed:
         * 1. Validation of the passed product's reference (database);
         * 
         * This method can return a soft-deleted product.
         */
        public ValidationOutput ClientGetByReference(string reference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!ExistsAndIsActive(reference))
            {
                validationOutput.AddError("Reference of product",
                    "No product with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            validationOutput.DesiredReturn = _mapper.Map<ProductDto>(_productRepository.GetByReference(reference));
            return validationOutput;
        }

        /**
         * Method that will return all products present in the system, each in the form of a DTO OR all the errors found when trying to do so.
         *
         * May return an empty list, indicating that there are no products in the system (yet).
         * This list will not include soft-deleted products.
         */
        public IEnumerable<ProductDto> GetAll()
        {
            List<ProductDto> productDtoList = new List<ProductDto>();
            List<Product> productList = _productRepository.List();

            //For-each just to convert each Product object into a ProductDto object
            foreach (var product in productList)
            {
                productDtoList.Add(_mapper.Map<ProductDto>(product));
            }

            return productDtoList;
        }


        // ============ Methods to UPDATE something ============

        /**
         * Method that will update the product (name, description, price and slot definition), with the passed reference, with the data present in the passed DTO OR return all the errors found when trying to do so.
         *
         * Validations performed:
         * 1. Validation of the passed products's reference (database);
         * 2. Validation of the name, description, price and slot definition of the passed DTO.
         */
        public ValidationOutput Update(string reference, ProductDto dto)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!ProductExists(reference))
            {
                validationOutput.AddError("Reference of product",
                    "No product with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            validationOutput = new ValidationOutputForbidden();
            if (dto.Reference != null)
            {
                validationOutput.AddError("Reference of product", "It's not allowed to update reference.");
                return validationOutput;
            }

            //2.
            validationOutput = _productDTOValidator.DTOIsValidForUpdate(dto);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            Product productToUpdate = _productRepository.GetByReference(reference);

            if (!string.IsNullOrEmpty(dto.CategoryReference) && !CategoryExists(dto.CategoryReference))
            {
                validationOutput.AddError("Reference of category",
                    "No category with the reference '" + dto.CategoryReference + "' exists in the system.");
                return validationOutput;
            }

            if (dto.Name != null)
            {
                productToUpdate.Name = dto.Name;
            }

            if (dto.Description != null)
            {
                productToUpdate.Description = dto.Description;
            }

            if (dto.Price != null)
            {
                productToUpdate.Price = _mapper.Map<Price>(dto.Price);
            }

            if (dto.SlotDefinition != null)
            {
                productToUpdate.SlotDefinition = _mapper.Map<SlotDefinition>(dto.SlotDefinition);
            }

            if (dto.CategoryReference != null)
            {
                productToUpdate.CategoryReference = dto.CategoryReference;
            }

            if (dto.ModelGroup != null)
            {
                productToUpdate.ModelGroup = _mapper.Map<ModelGroup>(dto.ModelGroup);
            }

            validationOutput.DesiredReturn = _mapper.Map<ProductDto>(_productRepository.Update(productToUpdate));
            return validationOutput;
        }


        // ============ Methods to REMOVE something ============

        /**
         * Method that will soft-delete the material with the passed reference OR return all the errors found when trying to do so.
         * 
         * Validations performed:
         * 1. Validation of the passed material's reference (database);
         */
        public ValidationOutput Remove(string reference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!ProductExists(reference))
            {
                validationOutput.AddError("Reference of product",
                    "No product with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            Product productToRemove = _productRepository.GetByReference(reference);

            _productRepository.Delete(productToRemove);
            return validationOutput;
        }


        // ============ Business Methods ============

        /**
         * Method that will add new materials and respective algorithms (in the form of ProductMaterial objects) to the product with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed product's reference (database);
         * 2. The received list has 1 or more elements;
         FOREACH RECEIVED ProductMaterial {
            * 3. Validation of the material's reference in current ProductMaterial (database);
            * 4. Verification if the material associated with current ProductMaterial is already associated with the product being considered.
                FOREACH RECEIVED MaterialAlgorithm {
                    * 5. Validation of current MaterialAlgorithm (business rules)
                    * 6. Validation for duplication between each MaterialAlgorithm received.
                }
            * 7. Validation for duplication between each ProductMaterial item received
         }
         */
        public ValidationOutput AddMaterialsAndRespectiveAlgorithms(string reference,
            IEnumerable<ProductMaterialDto> enumerableProductMaterialDto)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!ProductExists(reference))
            {
                validationOutput.AddError("Reference of product",
                    "No product with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            Product productToModify = _productRepository.GetByReference(reference);


            List<ProductMaterialDto> productMaterialDtoList =
                new List<ProductMaterialDto>(enumerableProductMaterialDto);

            validationOutput = new ValidationOutputBadRequest();
            //2.
            if (productMaterialDtoList.Count == 0)
            {
                validationOutput.AddError("Materials", "No materials were selected!");
                return validationOutput;
            }

            List<ProductMaterial> productMaterialListToAdd = new List<ProductMaterial>();

            foreach (var productMaterialDto in productMaterialDtoList)
            {
                //3.
                validationOutput = new ValidationOutputNotFound();
                if (!MaterialExists(productMaterialDto.MaterialReference))
                {
                    validationOutput.AddError("Reference of material",
                        "No material with the reference '" + productMaterialDto.MaterialReference +
                        "' exists in the system.");
                    return validationOutput;
                }

                //4.
                validationOutput = new ValidationOutputBadRequest();
                if (productToModify.IsAssociatedWithMaterial(productMaterialDto.MaterialReference))
                {
                    validationOutput.AddError("Reference of material",
                        "Material '" + productMaterialDto.MaterialReference + "' is already associated with product '" +
                        reference + "'.");
                    return validationOutput;
                }

                List<MaterialAlgorithmDto>
                    materialAlgorithmDtoListToAdd =
                        new List<MaterialAlgorithmDto>(); //Serves no purpose other than to assert if there are duplicates
                foreach (var materialAlgorithmDto in productMaterialDto.Algorithms)
                {
                    //5.
                    validationOutput = _algorithmDTOValidator.DTOIsValid(materialAlgorithmDto);
                    if (validationOutput.HasErrors())
                    {
                        return validationOutput;
                    }

                    //6.
                    validationOutput = new ValidationOutputBadRequest();
                    if (materialAlgorithmDtoListToAdd.Contains(materialAlgorithmDto))
                    {
                        validationOutput.AddError("Material algorithm",
                            "A received material algorithm between product'" + reference + "' and material '" +
                            productMaterialDto.MaterialReference +
                            "' is duplicated in the list of selected algorithms.");
                        return validationOutput;
                    }

                    materialAlgorithmDtoListToAdd.Add(materialAlgorithmDto);
                }

                ProductMaterial productMaterial = _mapper.Map<ProductMaterial>(productMaterialDto);

                validationOutput = new ValidationOutputBadRequest();
                //7.
                if (productMaterialListToAdd.Contains(productMaterial))
                {
                    validationOutput.AddError("Material",
                        "Material '" + productMaterialDto.MaterialReference +
                        "' and respective algorithms are duplicated in the list of materials (and chosen algorithms) selected!");
                    return validationOutput;
                }

                productMaterialListToAdd.Add(productMaterial);
            }

            foreach (var productMaterialToAdd in productMaterialListToAdd)
            {
                productMaterialToAdd.ProductReference = reference;
                productToModify.AddProductMaterial(productMaterialToAdd);
            }

            validationOutput.DesiredReturn = productMaterialListToAdd;
            _productRepository.Update(productToModify);
            return validationOutput;
        }

        /**
         * Method that will add delete materials and respective algorithms from the product with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed product's reference (database);
         * 2. The received list has 1 or more elements;
         FOREACH RECEIVED string (Material reference) {
            * 3. Verification if the material associated with current ProductMaterial is already associated with the product being considered.
            * 4. Validation for duplication between each material reference received
         }
         */
        public ValidationOutput DeleteMaterialsAndRespectiveAlgorithms(string reference,
            IEnumerable<MaterialDto> enumerableMaterialReference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!ProductExists(reference))
            {
                validationOutput.AddError("Reference of product",
                    "No product with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            Product productToModify = _productRepository.GetByReference(reference);

            List<MaterialDto> materialReferenceList = new List<MaterialDto>(enumerableMaterialReference);

            validationOutput = new ValidationOutputBadRequest();
            //2.
            if (materialReferenceList.Count == 0)
            {
                validationOutput.AddError("Materials", "No materials were selected!");
                return validationOutput;
            }

            List<MaterialDto> materialListToDelete = new List<MaterialDto>();

            foreach (var material in materialReferenceList)
            {
                //3.
                validationOutput = new ValidationOutputBadRequest();
                if (!productToModify.IsAssociatedWithMaterial(material.Reference))
                {
                    validationOutput.AddError("Reference of material",
                        "Material '" + material.Reference + "' is not associated with product '" + reference +
                        "'.");
                    return validationOutput;
                }

                validationOutput = new ValidationOutputBadRequest();
                //4.
                if (materialListToDelete.Contains(material))
                {
                    validationOutput.AddError("Material",
                        "Material '" + material.Reference +
                        "' and respective algorithms are duplicated in the list of materials (and chosen algorithms) selected!");
                    return validationOutput;
                }

                materialListToDelete.Add(material);
            }

            foreach (var materialToDelete in materialListToDelete)
            {
                productToModify.RemoveMaterialAndRespectiveAlgorithms(
                    _mapper.Map<Material>(materialToDelete));
            }

            _productRepository.Update(productToModify);
            return validationOutput;
        }

        /**
         * Method that will add new products and respective algorithms (in the form of Part objects) to the product with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed product's reference (database);
         * 2. The received list has 1 or more elements;
         FOREACH RECEIVED Part {
            * 3. Validation of the products's reference in each Part (database);
            * 4. Verification if the product associated with each Part received is already associated with the product being considered.
                FOREACH RECEIVED PartAlgorithm {
                    * 5. Validation of each PartAlgorithm (business rules)
                    * 6. Validation for duplication between each PartAlgorithm received
                }
            * 7. Validation for duplication between each Part object received
         }
         */
        public ValidationOutput AddProductsAndRespectiveAlgorithms(string reference,
            IEnumerable<PartDto> enumerablePartDto)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!ProductExists(reference))
            {
                validationOutput.AddError("Reference of product",
                    "No product with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            Product productToModify = _productRepository.GetByReference(reference);

            List<PartDto> partDtoList = new List<PartDto>(enumerablePartDto);

            validationOutput = new ValidationOutputBadRequest();
            //2.
            if (partDtoList.Count == 0)
            {
                validationOutput.AddError("Products", "No products (and respective algorithms) were selected!");
                return validationOutput;
            }

            List<Part> partListToAdd = new List<Part>();

            foreach (var partDto in partDtoList)
            {
                //3.
                validationOutput = new ValidationOutputNotFound();
                if (!ProductExists(partDto.ProductReference))
                {
                    validationOutput.AddError("Reference of product in a part",
                        "No product with the reference '" + partDto.ProductReference + "' exists in the system.");
                    return validationOutput;
                }

                //4.
                validationOutput = new ValidationOutputBadRequest();
                if (productToModify.IsAssociatedWithProduct(partDto.ProductReference))
                {
                    validationOutput.AddError("Reference of product in a part",
                        "Product '" + partDto.ProductReference + "' is already associated with main product '" +
                        reference + "'.");
                    return validationOutput;
                }

                List<PartAlgorithmDto>
                    partAlgorithmDtoListToAdd =
                        new List<PartAlgorithmDto>(); //Serves no purpose other than to assert if there are duplicates
                foreach (var partAlgorithmDto in partDto.Algorithms)
                {
                    //5.
                    validationOutput = _algorithmDTOValidator.DTOIsValid(partAlgorithmDto);
                    if (validationOutput.HasErrors())
                    {
                        return validationOutput;
                    }

                    //6.
                    validationOutput = new ValidationOutputBadRequest();
                    if (partAlgorithmDtoListToAdd.Contains(partAlgorithmDto))
                    {
                        validationOutput.AddError("Part algorithm",
                            "A received part algorithm between main product'" + reference + "' and product '" +
                            partDto.ProductReference + "' is duplicated.");
                        return validationOutput;
                    }

                    partAlgorithmDtoListToAdd.Add(partAlgorithmDto);
                }

                Part part = _mapper.Map<Part>(partDto);

                validationOutput = new ValidationOutputBadRequest();
                //7.
                if (partListToAdd.Contains(part))
                {
                    validationOutput.AddError("Product",
                        "Product '" + partDto.ProductReference + "' and respective algorithms are duplicated.");
                    return validationOutput;
                }

                partListToAdd.Add(part);
            }

            foreach (var partToAdd in partListToAdd)
            {
                productToModify.AddPart(partToAdd);
            }

            validationOutput.DesiredReturn = enumerablePartDto;
            _productRepository.Update(productToModify);
            return validationOutput;
        }

        /**
         * Method that will delete products and respective algorithms from the product with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed product's reference (database);
         * 2. The received list has 1 or more elements;
         FOREACH RECEIVED string (Product Reference) {
            * 3. Verification if each product reference received is associated with the product being considered.
            * 4. Validation for duplication between each product reference received
         }
         */
        public ValidationOutput DeleteProductsAndRespectiveAlgorithms(string reference,
            IEnumerable<ProductDto> enumerableProductReference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!ProductExists(reference))
            {
                validationOutput.AddError("Reference of product",
                    "No product with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            Product productToModify = _productRepository.GetByReference(reference);

            List<ProductDto> productReferenceList = new List<ProductDto>(enumerableProductReference);

            validationOutput = new ValidationOutputBadRequest();
            //2.
            if (productReferenceList.Count == 0)
            {
                validationOutput.AddError("Products", "No products were selected!");
                return validationOutput;
            }

            List<ProductDto> productReferencesListToDelete = new List<ProductDto>();

            foreach (var productReference in productReferenceList)
            {
                //3.
                validationOutput = new ValidationOutputBadRequest();
                if (!productToModify.IsAssociatedWithProduct(productReference.Reference))
                {
                    validationOutput.AddError("Reference of product",
                        "Product '" + productReference + "' is not associated with the main product '" + reference +
                        "'.");
                    return validationOutput;
                }

                validationOutput = new ValidationOutputBadRequest();
                //4.
                if (productReferencesListToDelete.Contains(productReference))
                {
                    validationOutput.AddError("Product",
                        "Product '" + productReference + "' is duplicated in the list of selected products.");
                    return validationOutput;
                }

                productReferencesListToDelete.Add(productReference);
            }

            foreach (var productReferenceToDelete in productReferencesListToDelete)
            {
                productToModify.RemoveProductAndRespectiveAlgorithms(_mapper.Map<Product>(productReferenceToDelete));
            }

            _productRepository.Update(productToModify);
            return validationOutput;
        }

        /**
         * Method that will add new dimension values and respective algorithms (in the form of DimensionValues objects) to the product with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed product's reference (database);
         * 2. The received list has 1 or more elements;
         FOREACH RECEIVED DimensionValues {
            * 3. Validation of dimensions in each DimensionValues (business rules);
            * 4. Verification if each DimensionValues received is already associated with the product being considered.
                FOREACH RECEIVED DimensionAlgorithm {
                    * 5. Validation of each DimensionAlgorithm (business rules)
                    * 6. Validation for duplication between each DimensionAlgorithm received
                }
            * 7. Validation for duplication between each DimensionValues item received
         }
         */
        public ValidationOutput AddVariousDimensionValues(string reference,
            IEnumerable<DimensionValuesDto> enumerableDimensionValuesDto)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!ProductExists(reference))
            {
                validationOutput.AddError("Reference of product",
                    "No product with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            Product productToModify = _productRepository.GetByReference(reference);

            List<DimensionValuesDto> dimensionValuesDtoList =
                new List<DimensionValuesDto>(enumerableDimensionValuesDto);

            validationOutput = new ValidationOutputBadRequest();
            //2.
            if (dimensionValuesDtoList.Count == 0)
            {
                validationOutput.AddError("Dimension values", "No set of dimension values were defined!");
                return validationOutput;
            }

            List<DimensionValues> dimensionValuesListToAdd = new List<DimensionValues>();

            foreach (var dimensionValuesDto in dimensionValuesDtoList)
            {
                //3.
                validationOutput = _dimensionValuesDTOValidator.DTOIsValid(dimensionValuesDto);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }

                DimensionValues dimensionValues = _mapper.Map<DimensionValues>(dimensionValuesDto);

                //4.
                validationOutput = new ValidationOutputBadRequest();
                if (productToModify.IsAssociatedWithDimensionValues(dimensionValues))
                {
                    validationOutput.AddError("Dimension values",
                        "One set of the specified sets of dimension values is already associated with the product '" +
                        reference + "'.");
                    return validationOutput;
                }

                List<DimensionAlgorithmDto>
                    dimensionAlgorithmDtoList =
                        new List<DimensionAlgorithmDto>(); //Serves no purpose other than to assert if there are duplicates
                foreach (var dimensionAlgorithmDto in dimensionValuesDto.Algorithms)
                {
                    //5.
                    validationOutput = _algorithmDTOValidator.DTOIsValid(dimensionAlgorithmDto);
                    if (validationOutput.HasErrors())
                    {
                        return validationOutput;
                    }

                    //6.
                    validationOutput = new ValidationOutputBadRequest();
                    if (dimensionAlgorithmDtoList.Contains(dimensionAlgorithmDto))
                    {
                        validationOutput.AddError("Dimension algorithm",
                            "A dimension algorithm between a set of dimension values and the product'" + reference +
                            "' is duplicated.");
                        return validationOutput;
                    }

                    dimensionAlgorithmDtoList.Add(dimensionAlgorithmDto);
                }

                validationOutput = new ValidationOutputBadRequest();
                //7.
                if (dimensionValuesListToAdd.Contains(dimensionValues))
                {
                    validationOutput.AddError("Dimension values",
                        "A set of dimension values and respective algorithms are duplicated.");
                    return validationOutput;
                }

                dimensionValuesListToAdd.Add(dimensionValues);
            }

            foreach (var dimensionValuesToAdd in dimensionValuesListToAdd)
            {
                productToModify.AddDimensionValues(dimensionValuesToAdd);
            }

            validationOutput.DesiredReturn = enumerableDimensionValuesDto;
            _productRepository.Update(productToModify);
            return validationOutput;
        }

        /**
         * Method that will add delete dimension values and respective algorithms (will be received in the form of DimensionValues objects) from the product with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed product's reference (database);
         * 2. The received list has 1 or more elements;
         FOREACH RECEIVED DimensionValues {
            * 3. Verification if each DimensionValues received is already associated with the product being considered.
            * 4. Validation for duplication between each DimensionValues item received
         }
         */
        public ValidationOutput DeleteVariousDimensionValues(string reference,
            IEnumerable<DimensionValuesDto> enumerableDimensionValuesDto)
        {
            List<DimensionValuesDto> dimensionValuesDtoList =
                new List<DimensionValuesDto>(enumerableDimensionValuesDto);
            ValidationOutput validationOutput = new ValidationOutputBadRequest();

            Product productToModify = _productRepository.GetByReference(reference);

            //1.
            validationOutput = new ValidationOutputNotFound();
            if (!ProductExists(reference))
            {
                validationOutput.AddError("Reference of product",
                    "No product with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            //2.
            if (dimensionValuesDtoList.Count == 0)
            {
                validationOutput.AddError("Dimension values", "No set of dimension values were defined!");
                return validationOutput;
            }

            List<DimensionValues> dimensionValuesListToDelete = new List<DimensionValues>();

            foreach (var dimensionValuesDto in dimensionValuesDtoList)
            {
                DimensionValues dimensionValues = _mapper.Map<DimensionValues>(dimensionValuesDto);

                //3.
                validationOutput = new ValidationOutputBadRequest();
                if (!productToModify.IsAssociatedWithDimensionValues(dimensionValues))
                {
                    validationOutput.AddError("Dimension values",
                        "One set of the specified sets of dimension values is not associated with the product '" +
                        reference + "' and therefore.");
                    return validationOutput;
                }

                //4.
                if (dimensionValuesListToDelete.Contains(dimensionValues))
                {
                    validationOutput.AddError("Dimension values",
                        "A set of dimension values and respective algorithms are duplicated.");
                    return validationOutput;
                }

                dimensionValuesListToDelete.Add(dimensionValues);
            }

            foreach (var dimensionValuesToDelete in dimensionValuesListToDelete)
            {
                productToModify.RemoveDimensionValues(dimensionValuesToDelete);
            }

            _productRepository.Update(productToModify);
            return validationOutput;
        }

        /**
         * Method that will return all the products that can be chosen to complement the product with the passed reference.
         * 
         * Validations performed:
         * 1. Validation of the passed product's reference (database);
         * 
         */
        public ValidationOutput GetPartProductsAvailableToProduct(string reference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!ProductExists(reference))
            {
                validationOutput.AddError("Reference of product",
                    "No product with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            Product passedProduct = _productRepository.GetByReference(reference);

            List<ProductDto> productDtoList = new List<ProductDto>();

            foreach (var part in passedProduct.Parts)
            {
                Product currentPartProduct = _productRepository.GetByReference(part.ProductReference);
                productDtoList.Add(_mapper.Map<ProductDto>(currentPartProduct));
            }

            validationOutput.DesiredReturn = productDtoList;
            return validationOutput;
        }

        /**
         * Method that will return all products that belong to the category with the passed reference.
         * 
         * Validations performed:
         * 1. Validation of the passed category's reference (database);
         * 
         */
        public ValidationOutput GetProductsOfCategory(string categoryReference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!CategoryExists(categoryReference))
            {
                validationOutput.AddError("Reference of category",
                    "No category with the reference '" + categoryReference + "' exists in the system.");
                return validationOutput;
            }

            List<ProductDto> productDtoList = new List<ProductDto>();
            List<Product> productList = _productRepository.ProductsOfCategory(categoryReference);

            //For-each just to convert each Category object into a CategoryDto object
            foreach (var prod in productList)
            {
                productDtoList.Add(_mapper.Map<ProductDto>(prod));
            }

            validationOutput.DesiredReturn = productDtoList;
            return validationOutput;
        }

        /**
         * Method that will return all the materials that can be chosen to complement the product with the passed reference.
         * 
         * Validations performed:
         * 1. Validation of the passed product's reference (database);
         * 
         */
        public ValidationOutput GetMaterialsAvailableToProduct(string reference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!ProductExists(reference))
            {
                validationOutput.AddError("Reference of product",
                    "No product with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            Product passedProduct = _productRepository.GetByReference(reference);

            List<MaterialDto> materialDtoList = new List<MaterialDto>();

            foreach (var materialPlusAlgorithms in passedProduct.ProductMaterialList)
            {
                Material currentMaterialPlusAlgorithmsMaterial =
                    _materialRepository.GetByReference(materialPlusAlgorithms.MaterialReference);
                materialDtoList.Add(_mapper.Map<MaterialDto>(currentMaterialPlusAlgorithmsMaterial));
            }

            validationOutput.DesiredReturn = materialDtoList;
            return validationOutput;
        }

        private ValidationOutput getParentProducts(string childReference)
        {
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            List<Product> products = _productRepository.ProductsAndParts();
            Dictionary<Product, Part> info = new Dictionary<Product, Part>();
            foreach (var product in products)
            {
                foreach (var part in product.Parts)
                {
                    if (string.Equals(part.ProductReference, childReference, StringComparison.Ordinal))
                    {
                        info.Add(product, part);
                        break;
                    }
                }
            }

            if (info.Count < 1)
            {
                validationOutput.AddError("Product's reference",
                    "There are no parent products with the referred product as part");
                return validationOutput;
            }

            validationOutput.DesiredReturn = info;
            return validationOutput;
        }

        private ValidationOutput GetAlgorithmByDTO(AlgorithmDto dto)
        {
            ValidationOutput validationOutput = _algorithmDTOValidator.DTOIsValid(dto);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            if (dto is MaterialFinishPartAlgorithmDto)
            {
                MaterialFinishPartAlgorithmDto algorithm = (MaterialFinishPartAlgorithmDto)dto;
                validationOutput.DesiredReturn = _mapper.Map<MaterialFinishPartAlgorithm>(algorithm);
            }

            if (dto is RatioAlgorithmDto)
            {
                RatioAlgorithmDto algorithm = (RatioAlgorithmDto)dto;
                validationOutput.DesiredReturn = _mapper.Map<RatioAlgorithm>(algorithm);
            }

            if (dto is SizePercentagePartAlgorithmDto)
            {
                SizePercentagePartAlgorithmDto algorithm = (SizePercentagePartAlgorithmDto)dto;
                validationOutput.DesiredReturn = _mapper.Map<SizePercentagePartAlgorithm>(algorithm);
            }

            return validationOutput;
        }

        public ValidationOutput AddRestriction(string refer, AlgorithmDto dto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            Product desiredProduct = _productRepository.GetByReference(refer);
            if (desiredProduct == null)
            {
                validationOutput = new ValidationOutputNotFound();
                validationOutput.AddError("Product's reference", "There are no products with the given reference");
                return validationOutput;
            }

            validationOutput = GetAlgorithmByDTO(dto);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            Algorithm toAdd = (Algorithm)validationOutput.DesiredReturn;
            if (toAdd is PartAlgorithm)
            {
                validationOutput = getParentProducts(refer);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }

                List<ProductDto> output = new List<ProductDto>();
                Dictionary<Product, Part> info = (Dictionary<Product, Part>)validationOutput.DesiredReturn;
                foreach (var product in info.Keys)
                {
                    if (!info[product].AddPartAlgorithm((PartAlgorithm)toAdd))
                    {
                        validationOutput.AddError("Part Algorithm", "Part Algorithm already exists");
                        return validationOutput;
                    }

                    _productRepository.Update(product);
                    output.Add(_mapper.Map<ProductDto>(product));
                }

                validationOutput.DesiredReturn = output;
            }

            if (toAdd is DimensionAlgorithm)
            {
                foreach (var dimension in desiredProduct.Dimensions)
                {
                    if (!dimension.AddDimensionAlgorithm((DimensionAlgorithm)toAdd))
                    {
                        validationOutput.AddError("Dimension Algorithm", "Dimension Algorithm already exists");
                        return validationOutput;
                    }
                }

                _productRepository.Update(desiredProduct);
                validationOutput.DesiredReturn = new List<ProductDto> { _mapper.Map<ProductDto>(desiredProduct) };
            }

            return validationOutput;
        }

        public ValidationOutput AddPartRestriction(string refer, PartDto dto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            Product desiredProduct = _productRepository.GetByReference(refer);
            if (desiredProduct == null)
            {
                validationOutput = new ValidationOutputNotFound();
                validationOutput.AddError("Product's reference", "There are no products with the given reference");
                return validationOutput;
            }

            Product productPart = _productRepository.GetByReference(dto.ProductReference);
            if (productPart == null)
            {
                validationOutput = new ValidationOutputNotFound();
                validationOutput.AddError("Product's reference", "There are no products with the given reference (part reference)");
                return validationOutput;
            }

            Part returnPart = new Part(dto.ProductReference);

            if (dto.Algorithms == null)
            {
                desiredProduct.AddPart(returnPart);
                _productRepository.Update(desiredProduct);
                validationOutput.DesiredReturn = (ProductDto)_mapper.Map<ProductDto>(desiredProduct);

                return validationOutput;
            }
            List<PartAlgorithmDto> algorithmToAdd = dto.Algorithms;

            //produto não tem parte, então adiciona partes e algoritmos ao produto
            if (!desiredProduct.Parts.Contains(_mapper.Map<Part>(returnPart)))
            {
                foreach (var algorithmDto in algorithmToAdd)
                {
                    PartAlgorithm algorithm = _mapper.Map<PartAlgorithm>(algorithmDto);
                    validationOutput = _algorithmDTOValidator.DTOIsValid(algorithmDto);
                    if (validationOutput.HasErrors())
                    {
                        return validationOutput;
                    }

                    if (!returnPart.AddPartAlgorithm(algorithm))
                    {
                        validationOutput.AddError("Part Algorithm", "Part Algorithm already exists");
                        return validationOutput;
                    }
                }
                desiredProduct.AddPart(returnPart);
            }
            else //produto já tem parte, apenas adiciona os novos algoritmos ao produto
            {
                foreach (var algorithmDto in algorithmToAdd)
                {
                    PartAlgorithm algorithm = _mapper.Map<PartAlgorithm>(algorithmDto);
                    validationOutput = _algorithmDTOValidator.DTOIsValid(algorithmDto);
                    if (validationOutput.HasErrors())
                    {
                        return validationOutput;
                    }

                    //alterar o parte do produto
                    if (!desiredProduct.getProductPart(returnPart.ProductReference).AddPartAlgorithm(algorithm))
                    {
                        validationOutput.AddError("Part Algorithm", "Part Algorithm already exists");
                        return validationOutput;
                    }
                }
            }

            _productRepository.Update(desiredProduct);
            validationOutput.DesiredReturn = (ProductDto)_mapper.Map<ProductDto>(desiredProduct);

            return validationOutput;
        }

        public ValidationOutput UpdateModelGroup(string refer, ModelGroupDto dto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (dto == null)
            {
                validationOutput.AddError("DTO", "Model Group is missing!");
                return validationOutput;
            }
            Product desiredProduct = _productRepository.GetByReference(refer);
            if (desiredProduct == null)
            {
                validationOutput = new ValidationOutputNotFound();
                validationOutput.AddError("Product's reference", "There are no products with the given reference");
                return validationOutput;
            }
            validationOutput = _modelGroupDTOValidator.DTOIsValid(dto);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }
            desiredProduct.ModelGroup = _mapper.Map<ModelGroup>(dto);
            validationOutput.DesiredReturn = _mapper.Map<ProductDto>(_productRepository.Update(desiredProduct));
            return validationOutput;
        }
    }
}