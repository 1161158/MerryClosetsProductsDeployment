using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MerryClosets.Models;
using MerryClosets.Models.DTO;
using MerryClosets.Models.DTO.DTOValidators;
using MerryClosets.Models.Material;
using MerryClosets.Repositories.Interfaces;
using MerryClosets.Services.Interfaces;
using MerryClosets.Utils;
using Microsoft.EntityFrameworkCore;

namespace MerryClosets.Services.EF
{
    public class MaterialService : IMaterialService
    {
        private readonly IMapper _mapper;

        private readonly IMaterialRepository _materialRepository;

        private readonly MaterialDTOValidator _materialDTOValidator;
        private readonly ColorDTOValidator _colorDTOValidator;
        private readonly FinishDTOValidator _finishDTOValidator;
        private readonly PriceHistoryDTOValidator _priceHistoryDTOValidator;

        public MaterialService(IMapper mapper, IMaterialRepository materialRepository,
            MaterialDTOValidator materialDTOValidator, ColorDTOValidator colorDTOValidator,
            FinishDTOValidator finishDTOValidator, PriceHistoryDTOValidator priceHistoryDTOValidator)
        {
            _mapper = mapper;
            _materialRepository = materialRepository;
            _materialDTOValidator = materialDTOValidator;
            _finishDTOValidator = finishDTOValidator;
            _colorDTOValidator = colorDTOValidator;
            _priceHistoryDTOValidator = priceHistoryDTOValidator;
        }

        /**
         * Private method used to verify the existence of material, through its unique reference.
         */
        private bool MaterialExists(string reference)
        {
            var material = _materialRepository.GetByReference(reference);
            return material != null;
        }

        private bool ExistsAndIsActive(string reference)
        {
            var  material = _materialRepository.GetByReference(reference);
            if (material != null && material.IsActive)
            {
                return true;
            }

            return false;
        }
        
        /**
         * Method that, if the Price attribute in the passed material is different from null, will add that price to the history of the received material.
         */
        private MaterialDto AddNewPriceToMaterialHistory(MaterialDto material)
        {
            if (material.Price != null)
            {
                material.PriceHistory.Clear();
                material.PriceHistory.Add(new PriceHistoryDto(material.Price));
            }

            return material;
        }

        /**
         * Method that, if the Price attribute in the passed finish is diferent from null, will add that price to the history of the respective finish.
         */
        private FinishDto AddNewPriceToFinishHistory(FinishDto finish)
        {
            if (finish != null)
            {
                finish.PriceHistory.Clear();
                finish.PriceHistory.Add(new PriceHistoryDto(finish.Price));
            }

            return finish;
        }

        /**
         * Validations performed:
         * 
         * 1. The received list has 1 or more elements.
         FOREACH RECEIVED Color{
            * 2. Validation of each color's definition (business rules);
            * 3. Validation of the existence of each Color received, in the material with the passed reference
            * 4. Validation for duplication between received colors
         }
         */
        private ValidationOutput PrivateAddColorsToMaterial(string reference, IEnumerable<ColorDto> enumerableColorDto)
        {
            List<ColorDto>
                listColorDto =
                    new List<ColorDto>(
                        enumerableColorDto); //Since we receive an IEnumerable, we need to have something concrete

            //1.
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (listColorDto.Count == 0)
            {
                validationOutput.AddError("Selected colors", "No colors were selected!");
                return validationOutput;
            }

            Material materialToModify = _materialRepository.GetByReference(reference);
            List<Color> colorsToAdd = new List<Color>();

            foreach (var currentColorDto in listColorDto)
            {
                validationOutput = new ValidationOutputBadRequest();

                //2.
                validationOutput = _colorDTOValidator.DTOReferenceIsValid(currentColorDto.HexCode);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }

                //3
                validationOutput = _colorDTOValidator.DTOIsValidForRegister(currentColorDto);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }

                validationOutput = _colorDTOValidator.DTOIsValid(currentColorDto);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }
                
                
                Color currentColor = _mapper.Map<Color>(currentColorDto);
                
                //4.
                if (materialToModify.ContainsColor(currentColor))
                {
                    validationOutput.AddError("Color",
                        "Color with the hex code '" + currentColor.HexCode + "' already exists in material '" +
                        reference + "'!");
                    return validationOutput;
                }
                
                //5.
                if (colorsToAdd.Contains(currentColor))
                {
                    validationOutput.AddError("Color",
                        "Color with the hex code '" + currentColor.HexCode +
                        "' is duplicated in the list of selected colors.");
                    return validationOutput;
                }

                colorsToAdd.Add(currentColor);
            }

            foreach (var color in colorsToAdd)
            {
                Color currentColor = _mapper.Map<Color>(color);
                materialToModify.AddColor(currentColor);
            }

            validationOutput.DesiredReturn = enumerableColorDto;
            _materialRepository.Update(materialToModify);
            return validationOutput;
        }

        /**
         * Validations performed:
         * 
         * 1. The received list has 1 or more elements.
         FOREACH RECEIVED Color{
            * 2. Validation of each color's definition (business rules);
            * 3. Validation of the existence of each Color received, in the material with the passed reference
            * 4. Validation for duplication between received colors
         }
         */
        private ValidationOutput PrivateAddColorsWithMaterial(IEnumerable<ColorDto> enumerableColorDto)
        {
            List<ColorDto>
                listColorDto =
                    new List<ColorDto>(
                        enumerableColorDto); //Since we receive an IEnumerable, we need to have something concrete

            //1.
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (listColorDto.Count == 0)
            {
                validationOutput.AddError("Selected colors", "No colors were selected!");
                return validationOutput;
            }

            List<ColorDto> colorsToAdd = new List<ColorDto>();

            foreach (var currentColorDto in listColorDto)
            {
                validationOutput = new ValidationOutputBadRequest();

                //2.
                validationOutput = _colorDTOValidator.DTOReferenceIsValid(currentColorDto.HexCode);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }

                validationOutput = _colorDTOValidator.DTOIsValid(currentColorDto);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }

                //3.
                validationOutput = _colorDTOValidator.DTOIsValidForRegister(currentColorDto);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }

                //4.
                if (colorsToAdd.Contains(currentColorDto))
                {
                    validationOutput.AddError("Color",
                        "Color with the hex code '" + currentColorDto.HexCode +
                        "' is duplicated in the list of selected colors.");
                    return validationOutput;
                }

                colorsToAdd.Add(currentColorDto);
            }

            return validationOutput;
        }

        /**
         * Validations performed:
         * 
         * 1. The received list has 1 or more elements.
         FOREACH FINISH RECEIVED {
            * 2. Validation of each finish's reference (business rules);
            * 3. Validation of each finish's definition (business rules);
            * 4. Validation of the existence of each finish received, in the material with the passed reference.
            * 5. Validation for duplication between received finishes.
         }
         */
        private ValidationOutput PrivateAddFinishesToMaterial(string reference,
            IEnumerable<FinishDto> enumerableFinishDto)
        {
            List<FinishDto>
                listFinishDto =
                    new List<FinishDto>(
                        enumerableFinishDto); //Since we receive an IEnumerable, we need to have something concrete

            //1.
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (listFinishDto.Count == 0)
            {
                validationOutput.AddError("Finishes selected", "No finishes were selected!");
                return validationOutput;
            }

            Material materialToModify = _materialRepository.GetByReference(reference);
            List<Finish> finishesToAdd = new List<Finish>();

            foreach (var currentFinishDto in listFinishDto)
            {
                validationOutput = new ValidationOutputBadRequest();

                //2.
                validationOutput = _finishDTOValidator.DTOReferenceIsValid(currentFinishDto.Reference);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }

                //3.
                validationOutput = _finishDTOValidator.DTOIsValidForRegister(currentFinishDto);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }

                Finish currentFinish = _mapper.Map<Finish>(currentFinishDto);

                //4.
                if (materialToModify.ContainsFinish(currentFinish))
                {
                    validationOutput.AddError("Finish",
                        "Finish with the reference '" + currentFinish.Reference + "' already exists in material '" +
                        reference + "'!");
                    return validationOutput;
                }

                //5.
                if (finishesToAdd.Contains(currentFinish))
                {
                    validationOutput.AddError("Finish",
                        "Finish with the reference '" + currentFinish.Reference +
                        "' is duplicated in the list of selected finishes.");
                    return validationOutput;
                }

                finishesToAdd.Add(currentFinish);
            }

            foreach (var finishToAdd in listFinishDto)
            {
                AddNewPriceToFinishHistory(finishToAdd);
                finishToAdd.IsActive = true;
                Finish currentFinish = _mapper.Map<Finish>(finishToAdd);
                materialToModify.AddFinish(currentFinish);
            }

            validationOutput.DesiredReturn = enumerableFinishDto;
            _materialRepository.Update(materialToModify);
            return validationOutput;
        }

        /**
         * Validations performed:
         * 
         * 1. The received list has 1 or more elements.
         FOREACH FINISH RECEIVED {
            * 2. Validation of each finish's reference (business rules);
            * 3. Validation of each finish's definition (business rules);
            * 4. Validation of the existence of each finish received, in the material with the passed reference.
            * 5. Validation for duplication between received finishes.
         }
         */
        private ValidationOutput PrivateAddFinishesWithMaterial(IEnumerable<FinishDto> enumerableFinishDto)
        {
            List<FinishDto>
                listFinishDto =
                    new List<FinishDto>(
                        enumerableFinishDto); //Since we receive an IEnumerable, we need to have something concrete

            //1.
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (listFinishDto.Count == 0)
            {
                validationOutput.AddError("Finishes selected", "No finishes were selected!");
                return validationOutput;
            }

            List<FinishDto> finishesToAdd = new List<FinishDto>();

            foreach (var currentFinishDto in listFinishDto)
            {
                validationOutput = new ValidationOutputBadRequest();

                //2.
                validationOutput = _finishDTOValidator.DTOReferenceIsValid(currentFinishDto.Reference);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }

                //3.
                validationOutput = _finishDTOValidator.DTOIsValidForRegister(currentFinishDto);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }

                //5.
                if (finishesToAdd.Contains(currentFinishDto))
                {
                    validationOutput.AddError("Finish",
                        "Finish with the reference '" + currentFinishDto.Reference +
                        "' is duplicated in the list of selected finishes.");
                    return validationOutput;
                }

                AddNewPriceToFinishHistory(currentFinishDto);
                finishesToAdd.Add(currentFinishDto);
            }

            return validationOutput;
        }

        // ============ Methods to CREATE something ============

        /**
         * Method that will validate and create a new material in the database.
         *
         * Validations performed:
         * 1. Validation of the new material's reference (business rules);
         * 2. Validation of the new material's reference (database);
         * 3. Validation of the received info. (name, description, colors, finishes) (business rules)
         */
        public ValidationOutput Register(MaterialDto dto)
        {
            //1.
            ValidationOutput validationOutput = _materialDTOValidator.DTOReferenceIsValid(dto.Reference);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            //2.
            validationOutput = new ValidationOutputBadRequest();
            if (MaterialExists(dto.Reference))
            {
                validationOutput.AddError("Reference of material",
                    "A material with the reference '" + dto.Reference + "' already exists in the system!");
                return validationOutput;
            }

            //3.
            validationOutput = _materialDTOValidator.DTOIsValidForRegister(dto);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            if (dto.Colors.Count > 0)
            {
                validationOutput = PrivateAddColorsWithMaterial(dto.Colors);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }
            }

            if (dto.Finishes.Count > 0)
            {
                validationOutput = PrivateAddFinishesWithMaterial(dto.Finishes);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }
            }

            //NOTA: Ainda que este método verifique se o atributo Price é != null, nós, aqui no Register, nunca deixamos que seja null devido às validações
            AddNewPriceToMaterialHistory(dto);

            foreach (var finish in dto.Finishes)
            {
                finish.IsActive = true;
            }

            //If we reached here then that means we can add the new material
            validationOutput.DesiredReturn =
                _mapper.Map<MaterialDto>(
                    _materialRepository.Add(_mapper.Map<Material>(dto)));
            return validationOutput;
        }

        // ============ Methods to GET something ============

        /**
         * Method that will return either the material in the form of a DTO that has the passed reference OR all the errors found when trying to do so.
         * 
         * Validations performed:
         * 1. Validation of the passed material's reference (database);
         * 
         * This method can return a soft-deleted material.
         */
        public ValidationOutput GetByReference(string reference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!MaterialExists(reference))
            {
                validationOutput.AddError("Reference of material",
                    "No material with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            Material materialToReturn = _materialRepository.GetByReference(reference);

            validationOutput.DesiredReturn = _mapper.Map<MaterialDto>(materialToReturn);
            return validationOutput;
        }
        
        /**
         * Method that will return either the material in the form of a DTO that has the passed reference OR all the errors found when trying to do so.
         * 
         * Validations performed:
         * 1. Validation of the passed material's reference (database);
         * 
         * This method can return a soft-deleted material.
         */
        public ValidationOutput ClientGetByReference(string reference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!ExistsAndIsActive(reference))
            {
                validationOutput.AddError("Reference of material",
                    "No material with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            Material materialToReturn = _materialRepository.GetByReference(reference);

            validationOutput.DesiredReturn = _mapper.Map<MaterialDto>(materialToReturn);
            return validationOutput;
        }

        /**
         * Method that will return all materials present in the system, each in the form of a DTO OR all the errors found when trying to do so.
         *
         * May return an empty list, indicating that there are no materials in the system (yet).
         * This list will not include soft-deleted materials.
         */
        public IEnumerable<MaterialDto> GetAll()
        {
            List<MaterialDto> materialDtoList = new List<MaterialDto>();
            List<Material> materialList = _materialRepository.List();

            //For-each just to convert each Material object into a MaterialDto object
            foreach (var mat in materialList)
            {
                materialDtoList.Add(_mapper.Map<MaterialDto>(mat));
            }

            return materialDtoList;
        }

        public ValidationOutput GetColors(string reference)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!MaterialExists(reference))
            {
                validationOutput.AddError("Reference of material",
                    "No material with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            Material materialToReturn = _materialRepository.GetByReference(reference);
            List<ColorDto> listColors = new List<ColorDto>();
            foreach (var color in materialToReturn.Colors)
            {
                listColors.Add(_mapper.Map<ColorDto>(color));
            }

            validationOutput.DesiredReturn = listColors;
            return validationOutput;
        }

        // ============ Methods to UPDATE something ============

        /**
         * Method that will update the material (name, description and price) with the passed reference OR return all the errors found when trying to do so.
         *
         * Validations performed:
         * 1. Validation of the passed material's reference (database);
         * 2. Validation of the name, description and price of the passed DTO.
         */
        public ValidationOutput Update(string reference, MaterialDto dto)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!MaterialExists(reference))
            {
                validationOutput.AddError("Reference of material",
                    "No material with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            validationOutput = new ValidationOutputForbidden();
            if (dto.Reference != null)
            {
                validationOutput.AddError("Reference of material", "It's not allowed to update reference.");
                return validationOutput;
            }
            
            //2.
            validationOutput = _materialDTOValidator.DTOIsValidForUpdate(dto);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            Material materialToUpdate = _materialRepository.GetByReference(reference);

            if (dto.Name != null)
            {
                materialToUpdate.Name = dto.Name;
            }

            if (dto.Description != null)
            {
                materialToUpdate.Description = dto.Description;
            }
            
            /*if (dto.Price != null)
            {
                materialToUpdate.AddPriceToHistory(new PriceHistory(_mapper.Map<Price>(dto.Price)));
            }*/

            validationOutput.DesiredReturn =
                _mapper.Map<MaterialDto>(_materialRepository.Update(materialToUpdate));
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
            if (!MaterialExists(reference))
            {
                validationOutput.AddError("Referece of material",
                    "No material with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            Material materialToRemove = _materialRepository.GetByReference(reference);

            _materialRepository.Delete(materialToRemove);
            return validationOutput;
        }

        // ============ Business Methods ============

        /**
         * Method that will add new colors to the material with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed material's reference (database);
         */
        public ValidationOutput AddColorsToMaterial(string reference, IEnumerable<ColorDto> enumerableColorDto)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!MaterialExists(reference))
            {
                validationOutput.AddError("Reference of material",
                    "No material with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            validationOutput = PrivateAddColorsToMaterial(reference, enumerableColorDto);
            return validationOutput;
        }

        /**
         * Method that will add new finishes to the material with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed material's reference (database);
         */
        public ValidationOutput AddFinishesToMaterial(string reference, IEnumerable<FinishDto> enumerableFinishDto)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!MaterialExists(reference))
            {
                validationOutput.AddError("Reference of material",
                    "No material with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            validationOutput = PrivateAddFinishesToMaterial(reference, enumerableFinishDto);
            return validationOutput;
        }

        /**
         * Method that will remove colors from the material with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed material's reference (database);
         * 2. The received list has 1 or more elements.
         FOREACH COLOR RECEIVED {
            * 3. Validation of the existence of each Color received, in the material with the passed reference.
            * 4. Validation for duplication between received colors.
         }
         */
        public ValidationOutput RemoveColorsFromMaterial(string reference, IEnumerable<ColorDto> enumerableColorDto)
        {
            List<ColorDto>
                listColorDto =
                    new List<ColorDto>(
                        enumerableColorDto); //Since we receive an IEnumerable, we need to have something concrete

            ValidationOutput validationOutput = new ValidationOutputBadRequest();

            //1.
            validationOutput = new ValidationOutputNotFound();
            if (!MaterialExists(reference))
            {
                validationOutput.AddError("Reference of material",
                    "No material with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            //2.
            if (listColorDto.Count == 0)
            {
                validationOutput.AddError("Selected colors", "No colors were selected!");
                return validationOutput;
            }

            Material materialToModify = _materialRepository.GetByReference(reference);
            List<Color> colorsToDelete = new List<Color>();

            foreach (var currentColorDto in listColorDto)
            {
                validationOutput = new ValidationOutputBadRequest();
                Color currentColor = _mapper.Map<Color>(currentColorDto);

                //3.
                if (!materialToModify.ContainsColor(currentColor))
                {
                    validationOutput.AddError("Color",
                        "Color with the hex code '" + currentColor.HexCode + "' does not exist in material '" +
                        reference + "'!");
                    return validationOutput;
                }

                //4.
                if (colorsToDelete.Contains(currentColor))
                {
                    validationOutput.AddError("Color",
                        "Color with the hex code '" + currentColorDto.HexCode +
                        "' is duplicated in the list of selected colors.");
                    return validationOutput;
                }


                colorsToDelete.Add(currentColor);
            }

            foreach (var colorToDelete in colorsToDelete)
            {
                materialToModify.RemoveColor(colorToDelete);
            }

            _materialRepository.Update(materialToModify);
            return validationOutput;
        }

        /**
         * Method that will remove finishes from the material with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed material's reference (database);
         * 2. The received list has 1 or more elements.
         FOREACH FINISH RECEIVED {
            * 3. Validation of the existence of each finish received, in the material with the passed reference.
            * 4. Validation for duplication between received finishes.
         }
         */
        public ValidationOutput RemoveFinishesFromMaterial(string reference, IEnumerable<FinishDto> enumerableFinishDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            List<FinishDto>
                listFinishDto =
                    new List<FinishDto>(
                        enumerableFinishDto); //Since we receive an IEnumerable, we need to have something concrete

            //1.
            validationOutput = new ValidationOutputNotFound();
            if (!MaterialExists(reference))
            {
                validationOutput.AddError("Reference of material",
                    "No material with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            //2.
            if (listFinishDto.Count == 0)
            {
                validationOutput.AddError("Finishes selected", "No finishes were selected!");
                return validationOutput;
            }

            Material materialToModify = _materialRepository.GetByReference(reference);
            List<Finish> finishesToDelete = new List<Finish>();

            foreach (var currentFinishDto in listFinishDto)
            {
                validationOutput = new ValidationOutputBadRequest();

                Finish currentFinish = _mapper.Map<Finish>(currentFinishDto);

                //3.
                if (!materialToModify.ContainsFinish(currentFinish))
                {
                    validationOutput.AddError("Finish",
                        "Finish with the reference '" + currentFinish.Reference + "' does not exist in material '" +
                        reference + "'!");
                    return validationOutput;
                }

                //4.
                if (finishesToDelete.Contains(currentFinish))
                {
                    validationOutput.AddError("Finish",
                        "Finish with the reference '" + currentFinish.Reference +
                        "' is duplicated in the list of selected finishes.");
                    return validationOutput;
                }

                finishesToDelete.Add(currentFinish);
            }

            foreach (var finishToDelete in finishesToDelete)
            {
                materialToModify.RemoveFinish(finishToDelete);
            }

            _materialRepository.Update(materialToModify);
            return validationOutput;
        }

        /**
         * Method that will add prices that the material with the passed reference will have, in the future.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 2. Validation of the passed material's reference (database);
         * 1. The received list has 1 or more elements.
         FOREACH PRICE HISTORY RECEIVED {
            * 3. Validation of each price history's definition (business rules);
            * 4. Validation of the existence of each price history received, in the material with the passed reference.
            * 5. Validation for duplication between received price history items.
         }
         */
        public ValidationOutput AddPriceHistoryItemsToMaterial(string reference,
            IEnumerable<PriceHistoryDto> enumerablePriceHistoryDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();

            List<PriceHistoryDto> listPriceHistoryDto = new List<PriceHistoryDto>(enumerablePriceHistoryDto);

            //1.
            validationOutput = new ValidationOutputNotFound();
            if (!MaterialExists(reference))
            {
                validationOutput.AddError("Reference of material",
                    "No material with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            //2.
            if (listPriceHistoryDto.Count == 0)
            {
                validationOutput.AddError("Price history items defined", "No price history items were defined!");
                return validationOutput;
            }

            Material materialToModify = _materialRepository.GetByReference(reference);
            List<PriceHistory> priceHistoryItemsToAdd = new List<PriceHistory>();

            validationOutput = new ValidationOutputBadRequest();
            foreach (var currentPriceHistoryDto in listPriceHistoryDto)
            {
                //3.
                validationOutput = _priceHistoryDTOValidator.DTOIsValid(currentPriceHistoryDto);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }

                PriceHistory currentPriceHistory = _mapper.Map<PriceHistory>(currentPriceHistoryDto);

                //4.
                if (materialToModify.ContainsPriceHistory(currentPriceHistory))
                {
                    validationOutput.AddError("Price history item",
                        "A price history item set to the date " + currentPriceHistory.Date + " with the price " +
                        currentPriceHistory.Price.Value + " has already been defined in material '" + reference + "'!");
                    return validationOutput;
                }

                //5.
                if (priceHistoryItemsToAdd.Contains(currentPriceHistory))
                {
                    validationOutput.AddError("Price history item",
                        "A price history item is duplicated in the list of defined price history items.");
                    return validationOutput;
                }

                priceHistoryItemsToAdd.Add(currentPriceHistory);
            }

            foreach (var priceHistoryItemToAdd in priceHistoryItemsToAdd)
            {
                materialToModify.AddPriceToHistory(priceHistoryItemToAdd);
            }

            validationOutput.DesiredReturn = enumerablePriceHistoryDto;
            _materialRepository.Update(materialToModify);
            return validationOutput;
        }

        /**
         * Method that will add prices that the finish with the passed reference in the material with, equally, the passed reference will have, in the future.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed material's reference (database);
         * 2. The received list has 1 or more elements.
         * 3. Validation of the passed finish's reference (existence in the material);
         FOREACH PRICE HISTORY RECEIVED {
            * 4. Validation of each price history's definition (business rules);
            * 6. Validation of the existence of each price history received, in the the finish of material with the passed reference.
            * 5. Validation for duplication between received price history items.
         }
         */
        public ValidationOutput AddPriceHistoryItemsToFinishOfMaterial(string materialReference, string finishReference,
            IEnumerable<PriceHistoryDto> enumerablePriceHistoryDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();

            List<PriceHistoryDto> listPriceHistoryDto = new List<PriceHistoryDto>(enumerablePriceHistoryDto);

            //1.
            validationOutput = new ValidationOutputNotFound();
            if (!MaterialExists(materialReference))
            {
                validationOutput.AddError("Reference of material",
                    "No material with the reference '" + materialReference + "' exists in the system.");
                return validationOutput;
            }

            //2.
            if (listPriceHistoryDto.Count == 0)
            {
                validationOutput.AddError("Price history items defined", "No price history items were defined!");
                return validationOutput;
            }

            Material materialToModify = _materialRepository.GetByReference(materialReference);

            //3.
            validationOutput = new ValidationOutputNotFound();
            if (!materialToModify.ContainsFinish(finishReference))
            {
                validationOutput.AddError("Reference of finish",
                    "No finish with the reference '" + finishReference + "' exists in the material '" +
                    materialReference + "'.");
                return validationOutput;
            }

            Finish finishToModify = materialToModify.GetFinish(finishReference);

            List<PriceHistory> priceHistoryItemsToAdd = new List<PriceHistory>();

            validationOutput = new ValidationOutputBadRequest();
            foreach (var currentPriceHistoryDto in listPriceHistoryDto)
            {
                //4.
                validationOutput = _priceHistoryDTOValidator.DTOIsValid(currentPriceHistoryDto);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }

                PriceHistory currentPriceHistory = _mapper.Map<PriceHistory>(currentPriceHistoryDto);

                //5.
                if (finishToModify.ContainsPriceHistory(currentPriceHistory))
                {
                    validationOutput.AddError("Price history item",
                        "A price history item set to the date " + currentPriceHistory.Date + " with the price " +
                        currentPriceHistory.Price.Value + " has already been defined in the finish '" +
                        finishReference +
                        "' present in the material '" + materialReference + "'!");
                    return validationOutput;
                }

                //6.
                if (priceHistoryItemsToAdd.Contains(currentPriceHistory))
                {
                    validationOutput.AddError("Price history item",
                        "A price history item is duplicated in the list of defined price history items.");
                    return validationOutput;
                }

                priceHistoryItemsToAdd.Add(currentPriceHistory);
            }

            foreach (var priceHistoryItemToAdd in priceHistoryItemsToAdd)
            {
                finishToModify.AddPriceToHistory(priceHistoryItemToAdd);
            }

            validationOutput.DesiredReturn = enumerablePriceHistoryDto;
            _materialRepository.Update(materialToModify);
            return validationOutput;
        }

        /**
         * Method that will remove price history from the material with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed material's reference (database);
         * 2. The received list has 1 or more elements.
         FOREACH PRICE HISTORY RECEIVED {
            * 3. Validation of the existence of each price history received, in the material with the passed dto.
            * 4. Validation for duplication between received price history.
            * 5. Validation that the date of the price history is future
         }
         */
        public ValidationOutput RemovePriceHistoryFromMaterial(string reference,
            IEnumerable<PriceHistoryDto> enumerableHistoryDto)
        {
            List<PriceHistoryDto>
                listPriceHistoryDto =
                    new List<PriceHistoryDto>(
                        enumerableHistoryDto); //Since we receive an IEnumerable, we need to have something concrete

            ValidationOutput validationOutput = new ValidationOutputBadRequest();

            //1.
            validationOutput = new ValidationOutputNotFound();
            if (!MaterialExists(reference))
            {
                validationOutput.AddError("Reference of material",
                    "No material with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            //2.
            if (listPriceHistoryDto.Count == 0)
            {
                validationOutput.AddError("Selected price history", "No price history were selected!");
                return validationOutput;
            }

            Material materialToModify = _materialRepository.GetByReference(reference);
            List<PriceHistory> priceHistoryToDelete = new List<PriceHistory>();

            foreach (var currentPriceHistoryDto in listPriceHistoryDto)
            {
                validationOutput = new ValidationOutputBadRequest();
                PriceHistory currentPriceHistory = _mapper.Map<PriceHistory>(currentPriceHistoryDto);

                //3.
                if (!materialToModify.ContainsPriceHistory(currentPriceHistory))
                {
                    validationOutput.AddError("Price History",
                        "Price History with the date '" + currentPriceHistory.Date + "' does not exist in material '" +
                        reference + "'!");
                    return validationOutput;
                }

                //5.
                if (currentPriceHistoryDto.Date.CompareTo(DateTime.Now) <= 0)
                {
                    validationOutput.AddError("Price History",
                        "Price History with the date '" + currentPriceHistory.Date +
                        "' can't be deleted.");
                    return validationOutput;
                }

                //4.
                if (priceHistoryToDelete.Contains(currentPriceHistory))
                {
                    validationOutput.AddError("Price History",
                        "Price History with the date '" + currentPriceHistory.Date +
                        "' is duplicated in the list of selected price history.");
                    return validationOutput;
                }

                priceHistoryToDelete.Add(currentPriceHistory);
            }

            foreach (var priceToDelete in priceHistoryToDelete)
            {
                materialToModify.RemovePriceHistory(priceToDelete);
            }

            _materialRepository.Update(materialToModify);
            return validationOutput;
        }

        /**
         * Method that will remove price history from the material with the passed reference.
         * It is assumed that a list with 1 or more objects is received.
         * 
         * Validations performed:
         * 1. Validation of the passed material's reference (database);
         * 2. The received list has 1 or more elements.
         * 3. Validation of the passed finish's reference (database);
         FOREACH PRICE HISTORY RECEIVED {
            * 4. Validation of the existence of each price history received, in the material with the passed dto.
            * 5. Validation for duplication between received price history.
            * 6. Validation that the date of the price history is future
         }
         */
        public ValidationOutput DeleteFinishPriceHistoryFromMaterial(string reference, string finishReference,
            IEnumerable<PriceHistoryDto> enumerableHistoryDto)
        {
            List<PriceHistoryDto>
                listPriceHistoryDto =
                    new List<PriceHistoryDto>(
                        enumerableHistoryDto); //Since we receive an IEnumerable, we need to have something concrete

            ValidationOutput validationOutput = new ValidationOutputBadRequest();

            //1.
            validationOutput = new ValidationOutputNotFound();
            if (!MaterialExists(reference))
            {
                validationOutput.AddError("Reference of material",
                    "No material with the reference '" + reference + "' exists in the system.");
                return validationOutput;
            }

            //2.
            if (listPriceHistoryDto.Count == 0)
            {
                validationOutput.AddError("Selected price history", "No price history were selected!");
                return validationOutput;
            }

            Material materialToModify = _materialRepository.GetByReference(reference);
            List<PriceHistory> priceHistoryToDelete = new List<PriceHistory>();

            //3. 
            if (!materialToModify.ContainsFinish(finishReference))
            {
                validationOutput.AddError("Finish",
                    "Finish with the reference '" + finishReference + "' does not exist in material '" +
                    reference + "'!");
                return validationOutput;
            }

            Finish finishToModify = materialToModify.GetFinish(finishReference);

            foreach (var currentPriceHistoryDto in listPriceHistoryDto)
            {
                validationOutput = new ValidationOutputBadRequest();
                PriceHistory currentPriceHistory = _mapper.Map<PriceHistory>(currentPriceHistoryDto);

                //4.
                if (!finishToModify.ContainsPriceHistory(currentPriceHistory))
                {
                    validationOutput.AddError("Price History",
                        "Price History with the date '" + currentPriceHistory.Date + "' does not exist in finish '" +
                        finishReference + "'!");
                    return validationOutput;
                }

                //5.
                if (priceHistoryToDelete.Contains(currentPriceHistory))
                {
                    validationOutput.AddError("Price History",
                        "Price History with the date '" + currentPriceHistory.Date +
                        "' is duplicated in the list of selected price history.");
                    return validationOutput;
                }

                //6.
                if (currentPriceHistoryDto.Date.CompareTo(DateTime.Now) < 0)
                {
                    validationOutput.AddError("Price History",
                        "Price History with the date '" + currentPriceHistory.Date +
                        "' can't be deleted.");
                    return validationOutput;
                }

                priceHistoryToDelete.Add(currentPriceHistory);
            }

            foreach (var priceToDelete in priceHistoryToDelete)
            {
                finishToModify.RemovePriceHistory(priceToDelete);
            }

            //Removes the old Finish
            materialToModify.RemoveFinish(materialToModify.GetFinish(finishReference));

            //Adds the new one
            materialToModify.AddFinish(finishToModify);


            _materialRepository.Update(materialToModify);
            return validationOutput;
        }
    }
}