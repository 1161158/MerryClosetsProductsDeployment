using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MerryClosets.Models;
using MerryClosets.Models.ConfiguredProduct;
using MerryClosets.Models.DTO;
using MerryClosets.Models.DTO.DTOValidators;
using MerryClosets.Models.Material;
using MerryClosets.Models.Product;
using MerryClosets.Models.Restriction;
using MerryClosets.Repositories.Interfaces;
using MerryClosets.Services.Interfaces;

namespace MerryClosets.Services.EF
{
    public class ConfiguredProductService : IConfiguredProductService
    {
        private readonly IMapper _mapper;

        private readonly IConfiguredProductRepository _configuredProductRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMaterialRepository _materialRepository;

        private readonly IProductService _productService;
        
        private readonly ConfiguredProductDTOValidator _configuredProductDTOValidator;

        public ConfiguredProductService(IMapper mapper, IProductService productService, IConfiguredProductRepository configuredProductRepository, IProductRepository productRepository, IMaterialRepository materialRepository, ConfiguredProductDTOValidator configuredProductDTOValidator)
        {
            _mapper = mapper;
            _productService = productService;
            _configuredProductRepository = configuredProductRepository;
            _productRepository = productRepository;
            _materialRepository = materialRepository;
            _configuredProductDTOValidator = configuredProductDTOValidator;
        }

        private void ValidateSlotDefinition(Product product, ChildConfiguredProductDto configuredDto, ValidationOutput validationOutput)
        {
            if (product.SlotDefinition != null)
            {
                if (configuredDto.ConfiguredSlots == null || configuredDto.ConfiguredSlots.Count < 1)
                {
                    validationOutput.AddError("Slots", "Configured Slots required");
                    return;
                }
                var sum = 0;
                List<string> references = new List<string>();
                foreach (var slot in configuredDto.ConfiguredSlots)
                {
                    if (!(slot.Size >= product.SlotDefinition.MinSize && slot.Size <= product.SlotDefinition.MaxSize))
                    {
                        validationOutput.AddError("Slot Size", "Slot Size (" + slot.Size + ") is not between min size (" + product.SlotDefinition.MinSize + ") and max size (" + product.SlotDefinition.MaxSize + ")!");
                        return;
                    }
                    if (references.Contains(slot.Reference))
                    {
                        validationOutput.AddError("Slot Reference", "Slot Reference repeated");
                        return;
                    }
                    references.Add(slot.Reference);
                    sum += slot.Size;
                }
                if (sum != configuredDto.ConfiguredDimension.Width)
                {
                    validationOutput.AddError("Sum of Slot Sizes", "The sum of Slot Sizes is not equal to configured dimension width!");
                }
            }
        }

        private void ValidateMaterial(ChildConfiguredProductDto configuredDto, ValidationOutput validationOutput)
        {
            var material = _materialRepository.GetByReference(configuredDto.ConfiguredMaterial.OriginMaterialReference);

            if (!material.ChosenColorIsValid(configuredDto.ConfiguredMaterial.ColorReference))
            {
                validationOutput.AddError("Chosen Color", "Material does not support the chosen color!");
            }
            Price finishPrice = material.ChosenFinishIsValid(configuredDto.ConfiguredMaterial.FinishReference);
            if (finishPrice == null)
            {
                validationOutput.AddError("Chosen Finish", "Material does not support the chosen finish!");
            }
            else
            {
                validationOutput.DesiredReturn = new Price[] { material.CurrentPrice(), finishPrice };
            }
        }

        private void ChildrenFits(string slotReference, ConfiguredProduct parent, ConfiguredProductDto childDto, ValidationOutput validationOutput)
        {
            if (!ChildrenHeightFits(slotReference, parent, childDto))
            {
                validationOutput.AddError("Configured Product's height", "Parent Configured Product's height is insufficient!");
            }
            if (!ChildrenWidthFits(slotReference, parent, childDto))
            {
                validationOutput.AddError("Configured Product's width", "Parent Configured Product's width is insufficient!");
            }
            if (!ChildrenDepthFits(slotReference, parent, childDto))
            {
                validationOutput.AddError("Configured Product's depth", "Parent Configured Product's depth is insufficient!");
            }
        }

        private bool ChildrenDepthFits(string slotReference, ConfiguredProduct parentDto, ConfiguredProductDto childDto)
        {
            return parentDto.ConfiguredDimension.Depth >= childDto.ConfiguredDimension.Depth;
        }

        private bool ChildrenHeightFits(string slotReference, ConfiguredProduct parentDto, ConfiguredProductDto childDto)
        {
            int sum = 0;
            foreach (var part in parentDto.Parts)
            {
                if (string.Equals(part.ChosenSlotReference, slotReference, StringComparison.Ordinal))
                {
                    sum += _configuredProductRepository.GetByReference(part.ConfiguredChildReference).ConfiguredDimension.Height;
                }
            }
            return parentDto.ConfiguredDimension.Height - sum >= childDto.ConfiguredDimension.Height;
        }

        private bool ChildrenWidthFits(string slotReference, ConfiguredProduct parentDto, ConfiguredProductDto childDto)
        {
            foreach (var slot in parentDto.ConfiguredSlots)
            {
                if (string.Equals(slot.Reference, slotReference, StringComparison.Ordinal))
                {
                    return slot.Size >= childDto.ConfiguredDimension.Width;
                }
            }
            return false;
        }

        private ValidationOutput ValidateConfiguredProduct(ChildConfiguredProductDto configuredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            var product = _productRepository.GetByReference(configuredDto.ProductReference);

            if (!product.ChosenDimensionDtoIsValid(configuredDto.ConfiguredDimension))
            {
                validationOutput.AddError("Configured Product's dimension", "Chosen Dimension does not belong to any possible dimension");
                return validationOutput;
            }

            if (!product.ConfiguredMaterialDtoExists(configuredDto.ConfiguredMaterial))
            {
                validationOutput.AddError("Configured Material's reference", "Product does not support the chosen material");
                return validationOutput;
            }

            ValidateMaterial(configuredDto, validationOutput);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            ValidateSlotDefinition(product, configuredDto, validationOutput);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            var parentConfiguredProduct = _configuredProductRepository.GetByReference(configuredDto.ParentReference);
            if (parentConfiguredProduct != null)
            {
                ChildrenFits(configuredDto.SlotReference, parentConfiguredProduct, configuredDto, validationOutput);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }
            }

            if (!ObeysRestrictions(configuredDto))
            {
                validationOutput.AddError("Restriction", "Configured Product does not obey to defined restrictions!");
            }
            validationOutput.DesiredReturn = new Price[] { product.Price, ((Price[])validationOutput.DesiredReturn)[1], ((Price[])validationOutput.DesiredReturn)[0] };
            return validationOutput;
        }

        /* verifica se obdece as restricoes ao primeiro conjunto de dimensoes que couber */
        private bool CheckDimensionAlgorithms(Product baseProduct, ConfiguredProductDto configuredProduct)
        {
            foreach (var dimensionValues in baseProduct.Dimensions)
            {
                if (dimensionValues.CheckIfItBelongs(configuredProduct.ConfiguredDimension.Width, configuredProduct.ConfiguredDimension.Height, configuredProduct.ConfiguredDimension.Depth))
                {
                    foreach (var restriction in dimensionValues.Algorithms)
                    {
                        Dictionary<string, Object> parameters = new Dictionary<string, Object>();
                        parameters.Add("height", configuredProduct.ConfiguredDimension.Height);
                        parameters.Add("depth", configuredProduct.ConfiguredDimension.Depth);
                        parameters.Add("width", configuredProduct.ConfiguredDimension.Width);
                        if (!restriction.validate(parameters))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private bool CheckPartAlgorithms(Product baseProduct, ConfiguredProductDto configuredProduct)
        {
            ConfiguredProduct parentConfigured = _configuredProductRepository.GetByReference(((ChildConfiguredProductDto)configuredProduct).ParentReference);
            Product parentProduct = _productRepository.GetByReference(parentConfigured.ProductReference);
            Part part = parentProduct.IsProductPart(baseProduct);
            if (part == null)
            {
                return false;
            }
            foreach (var algorithm in part.Algorithms)
            {
                Dictionary<string, Object> parameters = new Dictionary<string, Object>();
                parameters.Add("ParentConfiguredProduct", parentConfigured);
                parameters.Add("ChildConfiguredProduct", configuredProduct);
                if (!algorithm.validate(parameters))
                {
                    return false;
                }
            }
            return true;
            // foreach (var configuredPart in configuredProduct.Parts)
            // {
            //     ConfiguredProduct childConfiguredProduct = _configuredProductRepository.GetByReference(configuredPart.ConfiguredChildReference);
            //     Product baseProductOfChildConfiguredProduct = _productRepository.GetByReference(childConfiguredProduct.ProductReference);

            //     foreach (var part in baseProduct.Parts)
            //     {
            //         if (part.ProductReference == baseProductOfChildConfiguredProduct.Reference)
            //         {
            //             foreach (var restriction in part.Algorithms)
            //             {
            //                 Dictionary<string, Object> parameters = new Dictionary<string, Object>();
            //                 parameters.Add("FatherConfiguredProduct", configuredProduct);
            //                 parameters.Add("ChildConfiguredProduct", childConfiguredProduct);
            //                 parameters.Add("FatherBaseProduct", baseProduct);
            //                 parameters.Add("ChildBaseProduct", baseProductOfChildConfiguredProduct);

            //                 if (!restriction.validate(parameters))
            //                 {
            //                     return false;
            //                 }
            //             }
            //             break;
            //         }
            //     }
            // }
        }

        private bool ObeysRestrictions(ConfiguredProductDto configuredProduct)
        {
            Product baseProduct = _productRepository.GetByReference(configuredProduct.ProductReference);
            if (!CheckDimensionAlgorithms(baseProduct, configuredProduct))
            {
                return false;
            }
            if (((ChildConfiguredProductDto)configuredProduct).ParentReference != null)
            {
                if (configuredProduct is ChildConfiguredProductDto && !CheckPartAlgorithms(baseProduct, configuredProduct))
                {
                    return false;
                }
            }

            // foreach (var materialProd in baseProduct.ProductMaterialList)
            // {
            //     if (configuredProduct.ConfiguredMaterial.OriginMaterialReference.Equals(materialProd.MaterialReference))
            //     {
            //         if (materialProd.Algorithms != null)
            //         {
            //             foreach (var restriction in materialProd.Algorithms)
            //             {
            //                 Dictionary<string, Object> parameters = new Dictionary<string, Object>();

            //                 parameters.Add("ConfiguredMaterial", configuredProduct.ConfiguredMaterial);
            //                 if (!restriction.validate(parameters))
            //                 {
            //                     return false;
            //                 }
            //             }
            //         }
            //         break;
            //     }
            // }


            return true;
        }

        public IEnumerable<ConfiguredProductDto> GetAll()
        {
            List<ConfiguredProductDto> returnList = new List<ConfiguredProductDto>();
            List<ConfiguredProduct> list = _configuredProductRepository.List();
            foreach (var configuredProduct in list)
            {
                returnList.Add(_mapper.Map<ConfiguredProductDto>(configuredProduct));
            }

            return returnList;
        }

        private ValidationOutput DataExists(ChildConfiguredProductDto dto)
        {
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (_productRepository.GetByReference(dto.ProductReference) == null)
            {
                validationOutput.AddError("Origin Product", "There are no product with the given reference!");
                return validationOutput;
            }
            if (_materialRepository.GetByReference(dto.ConfiguredMaterial.OriginMaterialReference) == null)
            {
                validationOutput.AddError("Origin Material", "There are no material with the given reference!");
                return validationOutput;
            }

            if (dto.ParentReference != null)
            {
                if (_configuredProductRepository.GetByReference(dto.ParentReference) == null)
                {
                    validationOutput.AddError("Parent Configured Product", "There are no configured product with the given parent reference");
                    return validationOutput;
                }
                ConfiguredProduct cpd = _configuredProductRepository.GetByReference(dto.ParentReference);
                validationOutput = new ValidationOutputBadRequest();
                foreach (var slot in cpd.ConfiguredSlots)
                {
                    if (string.Equals(slot.Reference, dto.SlotReference, StringComparison.Ordinal))
                    {
                        var parentProduct = _productRepository.GetByReference(cpd.ProductReference);
                        foreach (var part in parentProduct.Parts)
                        {
                            if (string.Equals(part.ProductReference, dto.ProductReference))
                            {
                                return validationOutput;
                            }
                        }
                        validationOutput.AddError("Product Reference", "Parent Product does not support this product as child!");
                        return validationOutput;
                    }
                }
                validationOutput.AddError("Slot Reference", "The selected slot reference does not exists in parent configured product");
                return validationOutput;
            }
            return validationOutput;
        }

        /**
         * Method that will validate and create a new configured product in the database.
         *
         * Validations performed:
         * 1. Validation of the new configured product's reference (business rules);
         * 2. Validation of the new configured product's reference (database);
         * 3. Validation of the received info. (product reference, configured material, configured dimension) (business rules)
         * 4. Validation of the received info. (origin product, origin material, parent product, slot, dimensions, restrictions) (database)
         * 5. Validation of the received info. (everything fits)
         */
        public ValidationOutput Register(ConfiguredProductDto configuredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (configuredDto == null)
            {
                validationOutput.AddError("ConfiguredProduct", "Null reference.");
                return validationOutput;
            }

            var lastRef = _configuredProductRepository.ConfiguredProductsLenght();
            lastRef++;
            configuredDto.Reference = "confPro" + Convert.ToString(lastRef);
            
            //1.
            validationOutput = _configuredProductDTOValidator.DTOReferenceIsValid(configuredDto.Reference);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            //2
            if (_configuredProductRepository.GetByReference(configuredDto.Reference) != null)
            {
                validationOutput.AddError("ConfiguredProduct's reference", "A ConfiguredProduct with the reference '" + configuredDto.Reference + "' already exists in the system!");
                return validationOutput;
            }

            //3
            validationOutput = _configuredProductDTOValidator.DTOIsValidForRegister(configuredDto);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            ChildConfiguredProductDto ccpd = (ChildConfiguredProductDto)configuredDto;

            //4
            validationOutput = DataExists(ccpd);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            //5
            validationOutput = ValidateConfiguredProduct(ccpd);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            var configured = _mapper.Map<ConfiguredProduct>(configuredDto);

            if (_productRepository.GetByReference(configured.ProductReference).SlotDefinition == null || !configured.ConfiguredSlots.Any())
            {
                configured.ConfiguredSlots.Clear();
                configured.ConfiguredSlots.Add(new ConfiguredSlot(configured.ConfiguredDimension.Width, "default_slot_reference"));
            }

            configured.Price = new Price(CalculatePriceValue(((Price[])validationOutput.DesiredReturn)[2], ((Price[])validationOutput.DesiredReturn)[1], ((Price[])validationOutput.DesiredReturn)[0], configured.ConfiguredDimension));

            if (ccpd.ParentReference != null)
            {
                var parentConfigured = _configuredProductRepository.GetByReference(ccpd.ParentReference);
                parentConfigured.Parts.Add(new ConfiguredPart(ccpd.Reference, ccpd.SlotReference));
                parentConfigured.Price.Value += configured.Price.Value;
                _configuredProductRepository.Update(parentConfigured);
            }

            validationOutput.DesiredReturn = _mapper.Map<ConfiguredProductDto>(_configuredProductRepository.Add(configured));

            return validationOutput;
        }

        private float CalculatePriceValue(Price finishPrice, Price materialPrice, Price productPrice, ConfiguredDimension configuredDimension)
        {
            var height = Conversor.MilimeterToMeter(configuredDimension.Height);
            var width = Conversor.MilimeterToMeter(configuredDimension.Width);
            var depth = Conversor.MilimeterToMeter(configuredDimension.Depth);
            var totalArea = (2 * (height * depth)) + (2 * (width * depth)) + (height * width);
            return (totalArea * (materialPrice.Value + finishPrice.Value) + productPrice.Value);
        }

        public ValidationOutput GetByReference(string reference)
        {
            ValidationOutput validationOutput = _configuredProductDTOValidator.DTOReferenceIsValid(reference);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }
            ConfiguredProduct configuredProduct = _configuredProductRepository.GetByReference(reference);
            if (configuredProduct == null)
            {
                validationOutput = new ValidationOutputNotFound();
                validationOutput.AddError("Configured Product's reference", "There are no configured products with the given reference");
                return validationOutput;
            }
            validationOutput.DesiredReturn = _mapper.Map<ConfiguredProductDto>(configuredProduct);
            return validationOutput;
        }

        public ValidationOutput GetAllInfoByReference(string reference)
        {
            ValidationOutput validationOutput = _configuredProductDTOValidator.DTOReferenceIsValid(reference);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }
            ConfiguredProduct configuredProduct = _configuredProductRepository.GetByReference(reference);
            if (configuredProduct == null)
            {
                validationOutput = new ValidationOutputNotFound();
                validationOutput.AddError("Configured Product's reference", "There are no configured products with the given reference");
                return validationOutput;
            }

            var product = _mapper.Map<ProductOrderDto>(configuredProduct);
            foreach (var part in product.Parts)
            {
                GetAllByReference(part.ConfiguredChildReference, product);
            }
            validationOutput.DesiredReturn = product;
            return validationOutput;
        }

        public ValidationOutput GetAvailableProducts(string reference)
        {
            ValidationOutput validationOutput = _configuredProductDTOValidator.DTOReferenceIsValid(reference);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }
            ConfiguredProduct configuredProduct = _configuredProductRepository.GetByReference(reference);
            if (configuredProduct == null)
            {
                validationOutput = new ValidationOutputNotFound();
                validationOutput.AddError("Configured Product's reference", "There are no configured products with the given reference");
                return validationOutput;
            }

            List<ProductDto> productsDto = (List<ProductDto>) _productService
                .GetPartProductsAvailableToProduct(configuredProduct.ProductReference).DesiredReturn;
            List<Product> productsAvailable = new List<Product>();
            foreach (var dto in productsDto)
            {
                productsAvailable.Add(_mapper.Map<Product>(dto));
            }
            List<ProductDto> productsAvailableFinal = new List<ProductDto>();

            foreach (var product in productsAvailable)
            {
                if (ProductFitsConfigured(product.Dimensions, configuredProduct.ConfiguredDimension))
                {
                    productsAvailableFinal.Add(_mapper.Map<ProductDto>(product));
                }
                
            }

            validationOutput.DesiredReturn = productsAvailableFinal;
            return validationOutput;
        }

        private bool ProductFitsConfigured(List<DimensionValues> productDimensions,
            ConfiguredDimension configuredDimension)
        {
            foreach (var dimensions in productDimensions)
            {
                bool validateHeight = Validate(dimensions.PossibleHeights, configuredDimension.Height);
                bool validateWidth = Validate(dimensions.PossibleWidths, configuredDimension.Width);
                bool validateDepth = Validate(dimensions.PossibleDepths, configuredDimension.Depth);
                
                if (validateDepth && validateHeight && validateWidth)
                {
                    return true;
                }
            }
            return false;
        }
        
        private bool Validate(List<Values> list, int dimension)
        {
            foreach (var value in list)
            {
                if (value is DiscreteValue)
                {
                    var newValue = (DiscreteValue)value;
                    if (newValue.Value == dimension)
                    {
                        return true;
                    }
                }else if (value is ContinuousValue)
                {
                    var newValue = (ContinuousValue)value;
                    if (dimension >= newValue.MinValue && dimension >= newValue.MaxValue)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        private void GetAllByReference(string reference, ProductOrderDto product)
        {
            var child = _mapper.Map<ProductOrderDto>(_configuredProductRepository.GetByReference(reference));
            product.ListProducts.Add(child);
            foreach (var part in child.Parts)
            {
                GetAllByReference(part.ConfiguredChildReference, child);
            }
        }

        public ValidationOutput Remove(string reference)
        {
            ValidationOutput validationOutput = _configuredProductDTOValidator.DTOReferenceIsValid(reference);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }
            ConfiguredProduct configuredProduct = _configuredProductRepository.GetByReference(reference);
            if (configuredProduct == null)
            {
                validationOutput = new ValidationOutputNotFound();
                validationOutput.AddError("Configured Product's reference", "There are no configured products with the given reference");
                return validationOutput;
            }

            _configuredProductRepository.Delete(configuredProduct);

            return validationOutput;
        }

        public ConfiguredProductDto GetById(long id)
        {
            return _mapper.Map<ConfiguredProductDto>(_configuredProductRepository.GetById(id));
        }

        //não deve ser atualizado
        public ValidationOutput Update(string reference, ConfiguredProductDto dto)
        {
            return new ValidationOutputBadRequest();
        }

    }
}