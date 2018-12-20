using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MerryClosets.Models.Collection;
using MerryClosets.Models.DTO;
using MerryClosets.Models.DTO.DTOValidators;
using MerryClosets.Repositories.Interfaces;
using MerryClosets.Services.Interfaces;
using MerryClosets.Utils;
using Microsoft.EntityFrameworkCore;

namespace MerryClosets.Services.EF
{
    public class CatalogService : ICatalogService
    {
        private readonly IMapper _mapper;

        private readonly ICatalogRepository _catalogRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IConfiguredProductRepository _configuredProductRepository;

        private readonly CatalogDTOValidator _catalogDTOValidator;
        private readonly CollectionDTOValidator _collectionDTOValidator;
        private readonly ConfiguredProductDTOValidator _configuredProductDTOValidator;

        public CatalogService(IMapper mapper, ICatalogRepository catalogRepository, ICollectionRepository collectionRepository, IConfiguredProductRepository configuredProductRepository, CatalogDTOValidator catalogDTOValidator, CollectionDTOValidator collectionDTOValidator, ConfiguredProductDTOValidator configuredProductDTOValidator)
        {
            _mapper = mapper;
            _catalogRepository = catalogRepository;
            _collectionRepository = collectionRepository;
            _configuredProductRepository = configuredProductRepository;
            _catalogDTOValidator = catalogDTOValidator;
            _collectionDTOValidator = collectionDTOValidator;
            _configuredProductDTOValidator = configuredProductDTOValidator;
        }

        /**
         * Private method used to verify the existence of a catalog in the DB, through its unique reference.
         */
        private bool CatalogExists(string reference)
        {
            var category = _catalogRepository.GetByReference(reference);
            return category != null;
        }

        private bool ExistsAndIsActive(string reference)
        {
            var catalog = _catalogRepository.GetByReference(reference);
            if (catalog != null && catalog.IsActive)
            {
                return true;
            }

            return false;
        }

        /**
         * Private method used to verify the existence of a collection in the DB, through its unique reference.
         */
        private bool CollectionExists(string reference)
        {
            var collection = _collectionRepository.GetByReference(reference);
            return collection != null;
        }

        /**
         * Private method used to verify the existence of a configured product in the DB, through its unique reference.
         */
        private bool ConfiguredProductExists(string reference)
        {
            var confProd = _configuredProductRepository.GetByReference(reference);
            return confProd != null;
        }

        // ============ Methods to CREATE something ============

        /**
         * Method that will validate and create a new catalog in the database.
         *
         * Validations performed:
         * 1. Validation of the new catalog's reference (business rules);
         * 2. Validation of the new catalog's reference (database);
         * 3. Validation of the received info. (name, description) (business rules)
         * 4. The received CatalogProductCollection list has 1 or more elements.
         FOREACH RECEIVED CatalogProductCollection {
            * 5. Validation of the collection's reference in ProductCollection of current CatalogProductCollection (database);
            * 6. Validation of the configured product's reference in ProductCollection of current CatalogProductCollection (database);
            * 7. Validation to assert whether the indicated configured product actually belongs to the indicated collection (ProductCollection in current CatalogProductCollection)
            * 8. Validation for duplication between each CatalogProductCollection
         }
         */
        public ValidationOutput Register(CatalogDto dto)
        {
            //1.
            ValidationOutput validationOutput = _catalogDTOValidator.DTOReferenceIsValid(dto.Reference);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            //2.
            validationOutput = new ValidationOutputBadRequest();
            if (CatalogExists(dto.Reference))
            {
                validationOutput.AddError("Reference of catalog", "A catalog with the reference '" + dto.Reference + "' already exists in the system!");
                return validationOutput;
            }

            //3.
            validationOutput = _catalogDTOValidator.DTOIsValidForRegister(dto);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            //4.
            validationOutput = new ValidationOutputBadRequest();
            /*if (dto.CatalogProductCollectionList.Count == 0)
            {
                validationOutput.AddError("Selected 'configured product - collection' items", "No 'configured product - collection' items were selected!");
                return validationOutput;
            }*/
            if (dto.CatalogProductCollectionList.Count > 0)
            {
                List<ProductCollection> productCollectionListToAdd = new List<ProductCollection>();

                foreach (var currentCatalogProductCollectionDto in dto.CatalogProductCollectionList)
                {
                    ProductCollectionDto
                        productCollectionDto =
                            currentCatalogProductCollectionDto.ProductCollection; //Just to simplify the code

                    //5.
                    validationOutput = new ValidationOutputNotFound();
                    if (!CollectionExists(productCollectionDto.CollectionReference))
                    {
                        validationOutput.AddError("Reference of collection of a 'configured product - collection' item",
                            "No collection with the reference '" + productCollectionDto.CollectionReference +
                            "' exists in the system.");
                        return validationOutput;
                    }

                    //6.
                    if (!ConfiguredProductExists(productCollectionDto.ConfiguredProductReference))
                    {
                        validationOutput.AddError(
                            "Reference of configured product of a 'configured product - collection' item",
                            "No configured product with the reference '" +
                            productCollectionDto.ConfiguredProductReference + "' exists in the system.");
                        return validationOutput;
                    }

                    Collection currentCollection =
                        _collectionRepository.GetByReference(productCollectionDto.CollectionReference);

                    //7.
                    validationOutput = new ValidationOutputBadRequest();
                    if (!currentCollection.ConfiguredProductIsInCollection(productCollectionDto
                        .ConfiguredProductReference))
                    {
                        validationOutput.AddError("'Configured product - collection' item",
                            "The configured product with reference '" +
                            productCollectionDto.ConfiguredProductReference +
                            "' does not belong to the collection with reference '" +
                            productCollectionDto.ConfiguredProductReference + "'.");
                        return validationOutput;
                    }

                    ProductCollection currentProdCollection = _mapper.Map<ProductCollection>(productCollectionDto);

                    //8.
                    if (productCollectionListToAdd.Contains(currentProdCollection))
                    {
                        validationOutput.AddError("'Configured product - collection' item",
                            "'Configured product - collection' item with configured product '" +
                            currentProdCollection.ConfiguredProductReference +
                            "' that is associated with the collection '" + currentProdCollection.CollectionReference +
                            "' is duplicated in the list of selected 'configured product - collection' items!");
                        return validationOutput;
                    }

                    productCollectionListToAdd.Add(currentProdCollection);
                }
            }

            Catalog catalogToRegister = _mapper.Map<Catalog>(dto);
            validationOutput.DesiredReturn = _mapper.Map<CatalogDto>(_catalogRepository.Add(catalogToRegister));

            return validationOutput;
        }

        // ============ Methods to GET something ============

        /**
         * Method that will return either the catalog in the form of a DTO that has the passed reference OR all the errors found when trying to do so.
         * 
         * Validations performed:
         * 1. Validation of the passed catalog's reference (database);
         * 
         * This method can return a soft-deleted catalog.
         */
        public ValidationOutput GetByReference(string reference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!CatalogExists(reference))
            {
                validationOutput.AddError("Reference of catalog", "No catalog with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            validationOutput.DesiredReturn = _mapper.Map<CatalogDto>(_catalogRepository.GetByReference(reference));
            return validationOutput;
        }

        /**
         * Method that will return either the catalog in the form of a DTO that has the passed reference OR all the errors found when trying to do so.
         * 
         * Validations performed:
         * 1. Validation of the passed catalog's reference (database);
         * 
         * This method can return a soft-deleted catalog.
         */
        public ValidationOutput ClientGetByReference(string reference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!ExistsAndIsActive(reference))
            {
                validationOutput.AddError("Reference of catalog", "No catalog with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            validationOutput.DesiredReturn = _mapper.Map<CatalogDto>(_catalogRepository.GetByReference(reference));
            return validationOutput;
        }

        /**
         * Method that will return all catalogs present in the system, each in the form of a DTO OR all the errors found when trying to do so.
         *
         * May return an empty list, indicating that there are no catalogs in the system (yet).
         * This list will not include soft-deleted catalogs.
         */
        public IEnumerable<CatalogDto> GetAll()
        {
            List<CatalogDto> catalogDtoList = new List<CatalogDto>();
            List<Catalog> catalogList = _catalogRepository.List();

            //For-each just to convert each Category object into a CategoryDto object
            foreach (var catalog in catalogList)
            {
                catalogDtoList.Add(_mapper.Map<CatalogDto>(catalog));
            }

            return catalogDtoList;
        }

        // ============ Methods to UPDATE something ============

        /**
         * Method that will update the catalog (name and description), with the passed reference, with the data present in the passed DTO OR return all the errors found when trying to do so.
         *
         * Validations performed:
         * 1. Validation of the passed catalog's reference (database);
         * 2. Validation of the name and description of the passed DTO.
         */
        public ValidationOutput Update(string refer, CatalogDto dto)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!CatalogExists(refer))
            {
                validationOutput.AddError("Reference of catalog", "No catalog with the reference '" + refer + "' exists in the system.");
                return validationOutput;
            }

            validationOutput = new ValidationOutputForbidden();
            if (dto.Reference != null && !dto.Reference.Equals(refer))
            {
                validationOutput.AddError("Reference of catalog", "It's not allowed to update reference.");
                return validationOutput;
            }
            
            //2.
            validationOutput = _catalogDTOValidator.DTOIsValidForUpdate(dto);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            Catalog catalogToUpdate = _catalogRepository.GetByReference(refer);

            if (dto.Name != null)
            {
                catalogToUpdate.Name = dto.Name;
            }

            if (dto.Description != null)
            {
                catalogToUpdate.Description = dto.Description;
            }

            validationOutput.DesiredReturn = _mapper.Map<CatalogDto>(_catalogRepository.Update(catalogToUpdate));
            return validationOutput;
        }

        // ============ Methods to REMOVE something ============

        /**
         * Method that will soft-delete the catalog with the passed reference OR return all the errors found when trying to do so.
         * 
         * Validations performed:
         * 1. Validation of the passed catalog's reference (database);
         */
        public ValidationOutput Remove(string refer)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!CatalogExists(refer))
            {
                validationOutput.AddError("Reference of catalog", "No catalog with the reference '" + refer + "' exists in the system.");
                return validationOutput;
            }

            Catalog catalogToRemove = _catalogRepository.GetByReference(refer);

            _catalogRepository.Delete(catalogToRemove);
            return validationOutput;
        }

        // ============ Business Methods ============

        public ValidationOutput GetAllProductCollection(string reference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!CatalogExists(reference))
            {
                validationOutput.AddError("Reference of catalog", "No catalog with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }
            var catalog = _catalogRepository.GetByReference(reference);

            List<ProductCollectionDto> returnList = new List<ProductCollectionDto>();

            foreach (var catalogProductCollection in catalog.CatalogProductCollectionList)
            {
                var productCollection = catalogProductCollection.ProductCollection;
                returnList.Add(_mapper.Map<ProductCollectionDto>(productCollection));
            }
            validationOutput.DesiredReturn = returnList;
            return validationOutput;
        }

        /**
         * Method that will add new configured products, that belong to a certain, existing collection, (in the form of ProductCollection objects) to the catalog with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed catalog's reference (database);
         * 2. The received list has 1 or more elements.
         FOREACH RECEIVED ProductCollection {
            * 3. Validation of collection's reference in current ProductCollection (database);
            * 4. Validation of configured product's reference in current ProductCollection (database);
            * 5. Validation to assert whether the indicated configured product actually belongs to the indicated collection
            * 6. Validation of the existence of current ProductCollection received, in the catalog with the passed reference
            * 7. Validation for duplication between each ProductCollection received
         }
         */
        public ValidationOutput AddVariousProductCollection(string reference, IEnumerable<ProductCollectionDto> enumerableProductCollectionDto)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!CatalogExists(reference))
            {
                validationOutput.AddError("Reference of catalog", "No catalog with the '" + reference + "' exists in the system!");
                return validationOutput;
            }

            Catalog catalogToModify = _catalogRepository.GetByReference(reference);

            List<ProductCollectionDto> productCollectionDtoList = new List<ProductCollectionDto>(enumerableProductCollectionDto);
            //2.
            validationOutput = new ValidationOutputBadRequest();
            if (productCollectionDtoList.Count == 0)
            {
                validationOutput.AddError("'Configured product - collection' items", "No 'configured product - collection' items were selected!");
                return validationOutput;
            }

            List<ProductCollection> productCollectionListToAdd = new List<ProductCollection>();

            foreach (var currentProductCollectionDto in productCollectionDtoList)
            {
                //3.
                validationOutput = new ValidationOutputNotFound();
                if (!CollectionExists(currentProductCollectionDto.CollectionReference))
                {
                    validationOutput.AddError("Reference of collection of a selected 'configured product - collection' item", "No collection with the reference '" + currentProductCollectionDto.CollectionReference + "' exists in the system.");
                    return validationOutput;
                }

                //4.
                if (!ConfiguredProductExists(currentProductCollectionDto.ConfiguredProductReference))
                {
                    validationOutput.AddError("Reference of configured product of a selected 'configured product - collection' item", "No configured product with the reference '" + currentProductCollectionDto.ConfiguredProductReference + "' exists in the system.");
                    return validationOutput;
                }

                //5.
                validationOutput = new ValidationOutputBadRequest();
                Collection currentCollection = _collectionRepository.GetByReference(currentProductCollectionDto.CollectionReference);
                if (!currentCollection.ConfiguredProductIsInCollection(currentProductCollectionDto.ConfiguredProductReference))
                {
                    validationOutput.AddError("'Configured product - collection' item", "The configured product with reference '" + currentProductCollectionDto.ConfiguredProductReference + "' does not belong to the collection with reference '" + currentCollection.Reference + "'.");
                    return validationOutput;
                }

                ProductCollection currentProdCollection = _mapper.Map<ProductCollection>(currentProductCollectionDto);

                //5.
                if (catalogToModify.ContainsProductCollection(currentProdCollection))
                {
                    validationOutput.AddError("'Configured product - collection' item", "'Configured product - collection' item with configured product '" + currentProdCollection.ConfiguredProductReference + "' that is associated with the collection '" + currentProdCollection.CollectionReference + "' already belongs to catalog with reference '" + reference + "'");
                    return validationOutput;
                }

                //6.
                if (productCollectionListToAdd.Contains(currentProdCollection))
                {
                    validationOutput.AddError("'Configured product - collection' item", "'Configured product - collection' item with configured product '" + currentProdCollection.ConfiguredProductReference + "' that is associated with the collection '" + currentProdCollection.CollectionReference + "' is duplicated in the list of 'configured product - collection' items selected!");
                    return validationOutput;
                }

                productCollectionListToAdd.Add(currentProdCollection);
            }

            foreach (var productCollectionToAdd in productCollectionListToAdd)
            {
                catalogToModify.AddProductCollection(productCollectionToAdd);
            }

            validationOutput.DesiredReturn = enumerableProductCollectionDto;
            _catalogRepository.Update(catalogToModify);
            return validationOutput;
        }

        /**
         * Method that will delete configured products, that belong to a certain, existing collection, (in the form of ProductCollection objects) from the catalog with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed catalog's reference (database);
         * 2. The received list has 1 or more elements.
         FOREACH RECEIVED ProductCollection {
            * 3. Validation of the existence of current ProductCollection received, in the catalog with the passed reference
            * 4. Validation for duplication between each ProductCollection received
         }
         */
        public ValidationOutput DeleteVariousProductCollection(string reference, IEnumerable<ProductCollectionDto> enumerableProductCollectionDto)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!CatalogExists(reference))
            {
                validationOutput.AddError("Reference of catalog", "No catalog with the '" + reference + "' exists in the system!");
                return validationOutput;
            }

            Catalog catalogToModify = _catalogRepository.GetByReference(reference);

            List<ProductCollectionDto> productCollectionDtoList = new List<ProductCollectionDto>(enumerableProductCollectionDto);
            //2.
            validationOutput = new ValidationOutputBadRequest();
            if (productCollectionDtoList.Count == 0)
            {
                validationOutput.AddError("Selected 'configured product - collection' items", "No 'configured product - collection' items were selected!");
                return validationOutput;
            }

            List<ProductCollection> productCollectionListToRemove = new List<ProductCollection>();

            foreach (var currentProductCollectionDto in productCollectionDtoList)
            {
                ProductCollection currentProdCollection = _mapper.Map<ProductCollection>(currentProductCollectionDto);

                //3.
                if (!catalogToModify.ContainsProductCollection(currentProdCollection))
                {
                    validationOutput.AddError("'Configured product - collection' item", "'Configured product - collection' item with configured product '" + currentProdCollection.ConfiguredProductReference + "' that is associated with the collection '" + currentProdCollection.CollectionReference + "' does not belong to catalog with reference '" + reference + "'");
                    return validationOutput;
                }

                //4.
                if (productCollectionListToRemove.Contains(currentProdCollection))
                {
                    validationOutput.AddError("'Configured product - collection' item", "'Configured product - collection' item with configured product '" + currentProdCollection.ConfiguredProductReference + "' that is associated with the collection '" + currentProdCollection.CollectionReference + "' is duplicated in the list of 'configured product - collection' items selected!");
                    return validationOutput;
                }

                productCollectionListToRemove.Add(currentProdCollection);
            }

            foreach (var productCollectionToRemove in productCollectionListToRemove)
            {
                catalogToModify.RemoveProductCollection(productCollectionToRemove);
            }

            _catalogRepository.Update(catalogToModify);
            return validationOutput;
        }
    }
}