using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MerryClosets.Models;
using MerryClosets.Models.Collection;
using MerryClosets.Models.ConfiguredProduct;
using MerryClosets.Models.DTO;
using MerryClosets.Models.DTO.DTOValidators;
using MerryClosets.Repositories.Interfaces;
using MerryClosets.Services.Interfaces;
using MerryClosets.Utils;
using Microsoft.Extensions.Logging;

namespace MerryClosets.Services.EF
{
    public class CollectionService : ICollectionService
    {
        private readonly IMapper _mapper;

        private readonly ICollectionRepository _collectionRepository;
        private readonly IConfiguredProductRepository _configuredProductRepository;

        private readonly CollectionDTOValidator _collectionDTOValidator;
        private readonly ConfiguredProductDTOValidator _configuredProductDTOValidator;

        public CollectionService(IMapper mapper, ICollectionRepository collectionRepository, IConfiguredProductRepository configuredProductRepository, ILogger<CollectionService> logger, CollectionDTOValidator collectionDTOValidator, ConfiguredProductDTOValidator configuredProductDTOValidator)
        {
            _mapper = mapper;
            _collectionDTOValidator = collectionDTOValidator;
            _configuredProductRepository = configuredProductRepository;
            _configuredProductDTOValidator = configuredProductDTOValidator;
            _collectionRepository = collectionRepository;
        }

        /**
         * Private method used to verify the existence of a collection in the DB, through its unique reference.
         */
        private bool CollectionExists(string reference)
        {
            var collection = _collectionRepository.GetByReference(reference);
            return collection != null;
        }

        private bool ExistsAndIsActive(string reference)
        {
            var collection = _collectionRepository.GetByReference(reference);
            if (collection != null && collection.IsActive)
            {
                return true;
            }

            return false;
        }
        
        /**
         * Private method used to verify the existence of a configured product in the DB, through its unique reference.
         */
        private bool ConfiguredProductExists(string reference)
        {
            var confProduct = _configuredProductRepository.GetByReference(reference);
            return confProduct != null;
        }

        /**
         * Validations performed:
         * 
         * 1. The received list has 1 or more elements.
         FOREACH RECEIVED ConfiguredProductReference {
            * 2. Validation of each configured product reference (database);
            * 3. Validation of the existence of each configured product (represented by a reference) received, in the collection with the passed reference
            * 4. Validation for duplication between each configured product reference received
         }
         */

        // ============ Methods to CREATE something ============

        /**
         * Method that will validate and create a new collection in the database.
         *
         * Validations performed:
         * 1. Validation of the new collection's reference (business rules);
         * 2. Validation of the new collection's reference (database);
         * 3. Validation of the received info. (name, description, configured products) (business rules)
         * 4. The received list has 1 or more elements.
         FOREACH RECEIVED ConfiguredProductReference {
            * 5. Validation of each configured product reference (database);
            * 6. Validation for duplication between each configured product reference received
         }
         */
        public ValidationOutput Register(CollectionDto dto)
        {
            //1.
            ValidationOutput validationOutput = _collectionDTOValidator.DTOReferenceIsValid(dto.Reference);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            //2.
            if (CollectionExists(dto.Reference))
            {
                validationOutput.AddError("Reference of collection", "A collection with the reference '" + dto.Reference + "' already exists in the system!");
                return validationOutput;
            }

            //3.
            validationOutput = _collectionDTOValidator.DTOIsValidForRegister(dto);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            //4.
            validationOutput = new ValidationOutputBadRequest();
            /*if (dto.ProductCollectionList.Count == 0)
            {
                validationOutput.AddError("Selected configured products", "No configured products were selected!");
                return validationOutput;
            }*/
            if (dto.ProductCollectionList.Count > 0)
            {
                List<string> configuredProductReferenceListToAdd = new List<string>();

                foreach (var productCollectionDto in dto.ProductCollectionList)
                {
                    string configuredProductReference =
                        productCollectionDto.ConfiguredProductReference; //Just to simplify the code

                    //5.
                    if (!ConfiguredProductExists(configuredProductReference))
                    {
                        validationOutput.AddError("Reference of configured product",
                            "No configured product with the reference '" + configuredProductReference +
                            "' exists in the system.");
                        return validationOutput;
                    }

                    //6.
                    if (configuredProductReferenceListToAdd.Contains(configuredProductReference))
                    {
                        validationOutput.AddError("Configured product",
                            "Configured product '" + configuredProductReference +
                            "' is duplicated in the list of configured product selected!");
                        return validationOutput;
                    }

                    configuredProductReferenceListToAdd.Add(configuredProductReference);
                }
            }

            Collection collectionToRegister = _mapper.Map<Collection>(dto);
            validationOutput.DesiredReturn = _mapper.Map<CollectionDto>(_collectionRepository.Add(collectionToRegister));

            return validationOutput;
        }

        // ============ Methods to GET something ============

        /**
         * Method that will return either the collection in the form of a DTO that has the passed reference OR all the errors found when trying to do so.
         * 
         * Validations performed:
         * 1. Validation of the passed collection's reference (database);
         * 
         * This method can return a soft-deleted category.
         */
        public ValidationOutput GetByReference(string reference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!CollectionExists(reference))
            {
                validationOutput.AddError("Reference of collection", "No collection with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            validationOutput.DesiredReturn = _mapper.Map<CollectionDto>(_collectionRepository.GetByReference(reference));
            return validationOutput;
        }
        
        /**
         * Method that will return either the collection in the form of a DTO that has the passed reference OR all the errors found when trying to do so.
         * 
         * Validations performed:
         * 1. Validation of the passed collection's reference (database);
         * 
         * This method can return a soft-deleted category.
         */
        public ValidationOutput ClientGetByReference(string reference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!ExistsAndIsActive(reference))
            {
                validationOutput.AddError("Reference of collection", "No collection with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            validationOutput.DesiredReturn = _mapper.Map<CollectionDto>(_collectionRepository.GetByReference(reference));
            return validationOutput;
        }

        /**
         * Method that will return all collections present in the system, each in the form of a DTO OR all the errors found when trying to do so.
         *
         * May return an empty list, indicating that there are no collections in the system (yet).
         * This list will not include soft-deleted collections.
         */
        public IEnumerable<CollectionDto> GetAll()
        {
            List<CollectionDto> collectionDtoList = new List<CollectionDto>();
            List<Collection> collectionList = _collectionRepository.List();

            //For-each just to convert each Category object into a CategoryDto object
            foreach (var collection in collectionList)
            {
                collectionDtoList.Add(_mapper.Map<CollectionDto>(collection));
            }

            return collectionDtoList;
        }

        // ============ Methods to UPDATE something ============

        /**
         * Method that will update the collection (name and description), with the passed reference, with the data present in the passed DTO OR return all the errors found when trying to do so.
         *
         * Validations performed:
         * 1. Validation of the passed collection's reference (database);
         * 2. Validation of the name and description of the passed DTO.
         */
        public ValidationOutput Update(string reference, CollectionDto dto)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!CollectionExists(reference))
            {
                validationOutput.AddError("Reference of collection", "No collection with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }
            
            validationOutput = new ValidationOutputForbidden();
            if (dto.Reference != null)
            {
                validationOutput.AddError("Reference of collection", "It's not allowed to update reference.");
                return validationOutput;
            }

            //2.
            validationOutput = _collectionDTOValidator.DTOIsValidForUpdate(dto);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            Collection collectionToUpdate = _collectionRepository.GetByReference(reference);

            if (dto.Name != null)
            {
                collectionToUpdate.Name = dto.Name;
            }

            if (dto.Description != null)
            {
                collectionToUpdate.Description = dto.Description;
            }

            validationOutput.DesiredReturn = _mapper.Map<CollectionDto>(_collectionRepository.Update(collectionToUpdate));
            return validationOutput;

        }

        // ============ Methods to REMOVE something ============

        /**
         * Method that will soft-delete the collection with the passed reference OR return all the errors found when trying to do so.
         * 
         * Validations performed:
         * 1. Validation of the passed collection's reference (database);
         */
        public ValidationOutput Remove(string reference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!CollectionExists(reference))
            {
                validationOutput.AddError("Reference of collection", "No collection with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            Collection collectionToRemove = _collectionRepository.GetByReference(reference);
            _collectionRepository.Delete(collectionToRemove);
            return validationOutput;
        }

        // ============ Business Methods ============

        public ValidationOutput GetProductsCollection(string reference){
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!CollectionExists(reference))
            {
                validationOutput.AddError("Reference of collection", "No collection with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }
            var collection = _collectionRepository.GetByReference(reference);

            List<ConfiguredProductDto> returnList = new List<ConfiguredProductDto>();

            foreach(var configuredProductCollection in collection.ProductCollectionList){
                var configuredProduct = _configuredProductRepository.GetByReference(configuredProductCollection.ConfiguredProductReference);
                returnList.Add(this._mapper.Map<ConfiguredProductDto>(configuredProduct));
            }
            validationOutput.DesiredReturn = returnList;
            return validationOutput;
        }

        /**
         * Method that will add new configured products to the collection with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed collection's reference (database);
         * 2. The received list has 1 or more elements.
         FOREACH RECEIVED string (Configured Product reference) {
            * 3. Validation of current configured product reference (database);
            * 4. Validation of the existence of current configured product in the collection with the passed reference
            * 5. Validation for duplication between each configured product reference received
         */
        public ValidationOutput AddConfiguredProducts(string reference, IEnumerable<ConfiguredProductDto> enumerableConfiguredProduct)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!CollectionExists(reference))
            {
                validationOutput.AddError("Reference of collection", "No collection with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            Collection collectionToModify = _collectionRepository.GetByReference(reference);

            List<ConfiguredProductDto> configuredProductList = new List<ConfiguredProductDto>(enumerableConfiguredProduct);
            //2.
            validationOutput = new ValidationOutputBadRequest();
            if (configuredProductList.Count == 0)
            {
                validationOutput.AddError("Selected configured products", "No configured products were selected!");
                return validationOutput;
            }

            List<string> configuredProductListToAdd = new List<string>();

            foreach (var configuredProduct in configuredProductList)
            {
                //3.
                if (!ConfiguredProductExists(configuredProduct.Reference))
                {
                    validationOutput.AddError("Reference of configured product", "No configured product with the reference '" + configuredProduct.Reference + "' exists in the system.");
                    return validationOutput;
                }

                //4.
                if (collectionToModify.ConfiguredProductIsInCollection(configuredProduct.Reference))
                {
                    validationOutput.AddError("Configured product", "Configured product '" + configuredProduct.Reference + "' already belongs to collection with reference '" + reference + "'.");
                    return validationOutput;
                }

                //5.
                if (configuredProductListToAdd.Contains(configuredProduct.Reference))
                {
                    validationOutput.AddError("Configured product", "Configured product '" + configuredProduct.Reference + "' is duplicated in the list of configured product selected!");
                    return validationOutput;
                }

                configuredProductListToAdd.Add(configuredProduct.Reference);
            }

            foreach (var configuredProductToAdd in configuredProductListToAdd)
            {
                collectionToModify.AddConfiguredProduct(configuredProductToAdd);
            }

            validationOutput.DesiredReturn = configuredProductListToAdd;
            _collectionRepository.Update(collectionToModify);
            return validationOutput;
        }

        /**
         * Method that will delete configured products from the collection with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed collection's reference (database);
         * 2. The received list has 1 or more elements.
         FOREACH RECEIVED string (Configured Product reference) {
            * 3. Validation of the existence of current configured product in the collection with the passed reference
            * 4. Validation for duplication between each configured product reference received
         */
        public ValidationOutput DeleteConfiguredProducts(string reference, IEnumerable<ConfiguredProductDto> enumerableConfiguredProduct)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!CollectionExists(reference))
            {
                validationOutput.AddError("Reference of collection", "No collection with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            Collection collectionToModify = _collectionRepository.GetByReference(reference);
            List<ConfiguredProductDto> configuredProductList = new List<ConfiguredProductDto>(enumerableConfiguredProduct);
            
            //2.
            validationOutput = new ValidationOutputBadRequest();
            if (configuredProductList.Count == 0)
            {
                validationOutput.AddError("Selected configured products", "No configured products were selected!");
                return validationOutput;
            }

            List<string> configuredProductListToRemove = new List<string>();

            foreach (var configuredProduct in configuredProductList)
            {
                //3.
                if (!collectionToModify.ConfiguredProductIsInCollection(configuredProduct.Reference))
                {
                    validationOutput.AddError("Configured product", "Configured product '" + configuredProduct.Reference + "' does not belong to collection with reference '" + reference + "'.");
                    return validationOutput;
                }

                //4.
                if (configuredProductListToRemove.Contains(configuredProduct.Reference))
                {
                    validationOutput.AddError("Configured product", "Configured product '" + configuredProduct.Reference + "' is duplicated in the list of configured product selected!");
                    return validationOutput;
                }

                configuredProductListToRemove.Add(configuredProduct.Reference);
            }

            foreach (var configuredProductToRemove in configuredProductListToRemove)
            {
                collectionToModify.RemoveConfiguredProduct(configuredProductToRemove);
            }

            _collectionRepository.Update(collectionToModify);
            return validationOutput;
        }
    }
}