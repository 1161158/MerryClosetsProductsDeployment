using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MerryClosets.Models.DTO;
using MerryClosets.Services.EF;
using MerryClosets.Services.Interfaces;
using MerryClosets.Models;
using MerryClosets.Models.Material;
using MerryClosets.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MerryClosets.Controllers.Material
{
    [Route("api/material")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;
        private readonly IProductService _productService;
        private readonly CustomLogger _logger;
        private readonly IUserValidationService _userValidationService;

        public MaterialController(IMaterialService materialService, ILogger<ICategoryService> logger,
            IProductService productService, IUserValidationService userValidationService)
        {
            _materialService = materialService;
            _productService = productService;
            _userValidationService = userValidationService;
            _logger = new CustomLogger(logger);
        }

        // ========= POST METHODS =========

        /**
         * POST method that will create a new material in the system.
         */
        [HttpPost]
        public async Task<IActionResult> CreateMaterial([FromHeader(Name="Authorization")] string authorization, [FromBody] MaterialDto materialDto)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }
            
            _logger.logInformation(LoggingEvents.PostItem, "Creating By Dto: {0}", materialDto.Reference);
            ValidationOutput validationOutput = _materialService.Register(materialDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.PostBadRequest, "Creating Material Failed: {0}",
                        ((ValidationOutputBadRequest) validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.PostNotFound, "Creating Material Failed: {0}",
                        ((ValidationOutputNotFound) validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.PostInternalError,
                    "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                MaterialDto newMaterialDto = (MaterialDto) validationOutput.DesiredReturn;
                _logger.logInformation(LoggingEvents.PostOk, "Creating Material Succeeded: {0}",
                    newMaterialDto.Reference);
                return CreatedAtRoute("GetMaterial", new {reference = newMaterialDto.Reference}, newMaterialDto);
            }
        }

        /*
         * POST method that will add new colors to the material with the passed reference.
         */
        [HttpPost("{reference}/colors")]
        public async Task<IActionResult> AddColors([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference,
            [FromBody] IEnumerable<ColorDto> enumerableColorDto)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }

            object[] array = new object[2];
            array[0] = reference;
            array[1] = EnumerableUtils.convert(enumerableColorDto);
            _logger.logInformation(LoggingEvents.PostItem, "Adding Colors By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _materialService.AddColorsToMaterial(reference, enumerableColorDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.PostBadRequest, "Adding Colors Failed: {0}",
                        ((ValidationOutputBadRequest) validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.PostNotFound, "Adding Colors Failed: {0}",
                        ((ValidationOutputNotFound) validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.PostInternalError,
                    "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.PostOk, "Adding Colors to Material Succeeded: {0}", array[1]);
                return Ok(validationOutput.DesiredReturn);
            }
        }

        /*
         * POST method that will add new finishes to the material with the passed reference.
         */
        [HttpPost("{reference}/finishes")]
        public async Task<IActionResult> AddFinishes([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference,
            [FromBody] IEnumerable<FinishDto> enumerableFinishDto)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }

            object[] array = new object[2];
            array[0] = reference;
            array[1] = EnumerableUtils.convert(enumerableFinishDto);
            _logger.logInformation(LoggingEvents.PostItem, "Adding Finishes By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _materialService.AddFinishesToMaterial(reference, enumerableFinishDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.PostBadRequest, "Adding Finishes Failed: {0}",
                        ((ValidationOutputBadRequest) validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.PostNotFound, "Adding Finishes Failed: {0}",
                        ((ValidationOutputNotFound) validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.PostInternalError,
                    "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.PostOk, "Adding Finishes to Material Succeeded: {0}", array[1]);
                return Ok(validationOutput.DesiredReturn);
            }
        }

        [HttpPost("{reference}/price-date-items")]
        public async Task<IActionResult> AddPriceDateItems([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference,
            [FromBody] IEnumerable<PriceHistoryDto> enumerablePriceHistory)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }

            object[] array = new object[2];
            array[0] = reference;
            array[1] = EnumerableUtils.convert(enumerablePriceHistory);
            _logger.logInformation(LoggingEvents.PostItem, "Adding Prices By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput =
                _materialService.AddPriceHistoryItemsToMaterial(reference, enumerablePriceHistory);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.PostBadRequest, "Adding Prices Failed: {0}",
                        ((ValidationOutputBadRequest) validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.PostNotFound, "Adding Prices Failed: {0}",
                        ((ValidationOutputNotFound) validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.PostInternalError,
                    "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.PostOk, "Adding Prices to Material Succeeded: {0}", array[1]);
                return Ok(validationOutput.DesiredReturn);
            }
        }

        [HttpPost("{materialReference}/finishes/{finishReference}/price-date-items")]
        public async Task<IActionResult> AddPriceDateItems([FromHeader(Name="Authorization")] string authorization, [FromRoute] string materialReference, string finishReference,
            [FromBody] IEnumerable<PriceHistoryDto> enumerablePriceHistory)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }

            object[] array = new object[3];
            array[0] = materialReference;
            array[1] = finishReference;
            array[2] = EnumerableUtils.convert(enumerablePriceHistory);
            _logger.logInformation(LoggingEvents.PostItem, "Adding Prices to Finish By Reference: {0} - {1} -- {2}",
                array);
            ValidationOutput validationOutput =
                _materialService.AddPriceHistoryItemsToFinishOfMaterial(materialReference, finishReference,
                    enumerablePriceHistory);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.PostBadRequest, "Adding Prices to Finish Failed: {0}",
                        ((ValidationOutputBadRequest) validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.PostNotFound, "Adding Prices to Finish Failed: {0}",
                        ((ValidationOutputNotFound) validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.PostInternalError,
                    "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.PostOk, "Adding Prices to Finish of Material Succeeded: {0}",
                    array[2]);
                return Ok(validationOutput.DesiredReturn);
            }
        }

        // ========= GET METHODS =========

        /**
         * GET method that will return all existent materials in the system.
         */
        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<MaterialDto> list = _materialService.GetAll();
            _logger.logInformation(LoggingEvents.GetAllOk, "Getting All Materials: {0}", EnumerableUtils.convert(list));
            return Ok(list);
        }

        [HttpGet("{reference}/colors", Name = "GetMaterialColors")]
        public IActionResult GetMaterialColors([FromRoute] string reference)
        {
            _logger.logInformation(LoggingEvents.GetItem, "Getting Colors By Reference: {0}", reference);
            ValidationOutput validationOutput = _materialService.GetColors(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.GetItemBadRequest, "Getting Colors Material Failed: {0}",
                        ((ValidationOutputBadRequest) validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.GetItemNotFound, "Getting Colors Material Failed: {0}",
                        ((ValidationOutputNotFound) validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.GetItemInternalError,
                    "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.GetItemOk, "Getting Colors Material: {0}", reference);
                return Ok((List<ColorDto>) validationOutput.DesiredReturn);
            }
        }

        /**
         * GET method that will return the material with the given reference.
         */
        [HttpGet("{reference}", Name = "GetMaterial")]
        public IActionResult GetMaterial([FromRoute] string reference)
        {
            _logger.logInformation(LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
            ValidationOutput validationOutput = _materialService.GetByReference(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.GetItemBadRequest, "Getting Material Failed: {0}",
                        ((ValidationOutputBadRequest) validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.GetItemNotFound, "Getting Material Failed: {0}",
                        ((ValidationOutputNotFound) validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.GetItemInternalError,
                    "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.GetItemOk, "Getting Material: {0}",
                    ((MaterialDto) validationOutput.DesiredReturn).ToString());
                return Ok((MaterialDto) validationOutput.DesiredReturn);
            }
        }

        // ========= PUT METHODS =========

        /**
         * PUT method that will update the name, the description and the price of the material with the passed reference.
         */
        [HttpPut("{reference}")]
        public async Task<IActionResult> UpdateMaterial([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference, [FromBody] MaterialDto materialDto)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }

            _logger.logInformation(LoggingEvents.UpdateItem, "Updating By Reference: {0}", reference);
            ValidationOutput validationOutput = _materialService.Update(reference, materialDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.UpdateBadRequest, "Updating Failed: {0}",
                        ((ValidationOutputBadRequest) validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.UpdateNotFound, "Updating Failed: {0}",
                        ((ValidationOutputNotFound) validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.UpdateInternalError,
                    "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.UpdateNoContent, "Updating Material: {0}",
                    ((MaterialDto) validationOutput.DesiredReturn).ToString());
                _logger.logInformation(LoggingEvents.UpdateOk, "Updating Material: {0}",
                    ((MaterialDto) validationOutput.DesiredReturn).ToString());
                return NoContent();
            }
        }

        // ========= DELETE METHODS =========

        [HttpDelete("{reference}")]
        public async Task<IActionResult> DeleteMaterial([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }

            _logger.logInformation(LoggingEvents.SoftDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput = _materialService.Remove(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.DeleteBadRequest, "Deleting Failed: {0}",
                        ((ValidationOutputBadRequest) validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.DeleteNotFound, "Deleting Failed: {0}",
                        ((ValidationOutputNotFound) validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.DeleteInternalError,
                    "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.DeleteNoContent, "Deleting Material: {0}", reference);
                _logger.logInformation(LoggingEvents.SoftDeleteOk, "Deleting Material: {0}", reference);
                return NoContent();
            }
        }

        [HttpDelete("{reference}/colors")]
        public async Task<IActionResult> DeleteColorsFromMaterial([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference,
            [FromBody] IEnumerable<ColorDto> enumerableColorDto)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }

            _logger.logInformation(LoggingEvents.HardDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput =
                _materialService.RemoveColorsFromMaterial(reference, enumerableColorDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.DeleteBadRequest, "Deleting Failed: {0}",
                        ((ValidationOutputBadRequest) validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.DeleteNotFound, "Deleting Failed: {0}",
                        ((ValidationOutputNotFound) validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.DeleteInternalError,
                    "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.DeleteNoContent, "Deleting Colors From Material: {0}",
                    EnumerableUtils.convert(enumerableColorDto));
                _logger.logInformation(LoggingEvents.HardDeleteOk, "Deleting Colors From Material: {0}",
                    EnumerableUtils.convert(enumerableColorDto));
                return NoContent();
            }
        }

        [HttpDelete("{reference}/finishes")]
        public async Task<IActionResult> DeleteFinishesFromMaterial([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference,
            [FromBody] IEnumerable<FinishDto> enumerableFinishDto)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }

            _logger.logInformation(LoggingEvents.HardDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput =
                _materialService.RemoveFinishesFromMaterial(reference, enumerableFinishDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.DeleteBadRequest, "Deleting Failed: {0}",
                        ((ValidationOutputBadRequest) validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.DeleteNotFound, "Deleting Failed: {0}",
                        ((ValidationOutputNotFound) validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.DeleteInternalError,
                    "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.DeleteNoContent, "Deleting Finishes From Material: {0}",
                    EnumerableUtils.convert(enumerableFinishDto));
                _logger.logInformation(LoggingEvents.HardDeleteOk, "Deleting Finishes From Material: {0}",
                    EnumerableUtils.convert(enumerableFinishDto));
                return NoContent();
            }
        }

        [HttpDelete("{reference}/price-date-items")]
        public async Task<IActionResult> DeletePriceHistoryFromMaterial([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference,
            [FromBody] IEnumerable<PriceHistoryDto> enumerablePriceHistoryDto)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }

            _logger.logInformation(LoggingEvents.HardDeleteItem, "Deleting Price History By Reference: {0}", reference);
            ValidationOutput validationOutput =
                _materialService.RemovePriceHistoryFromMaterial(reference, enumerablePriceHistoryDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.DeleteBadRequest, "Deleting Failed: {0}",
                        ((ValidationOutputBadRequest) validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.DeleteNotFound, "Deleting Failed: {0}",
                        ((ValidationOutputNotFound) validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.DeleteInternalError,
                    "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.DeleteNoContent, "Deleting Price History From Material: {0}",
                    EnumerableUtils.convert(enumerablePriceHistoryDto));
                _logger.logInformation(LoggingEvents.HardDeleteOk, "Deleting Price History From Material: {0}",
                    EnumerableUtils.convert(enumerablePriceHistoryDto));
                return NoContent();
            }
        }

        [HttpDelete("{reference}/finishes/{finishReference}/price-date-items")]
        public async Task<IActionResult> DeleteFinishPriceHistoryFromMaterial([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference,
            [FromRoute] string finishReference,
            [FromBody] IEnumerable<PriceHistoryDto> enumerablePriceHistoryDto)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }

            _logger.logInformation(LoggingEvents.HardDeleteItem,
                "Deleting Price History Of Finish {0} of Material By Reference: {1}", finishReference, reference);
            ValidationOutput validationOutput =
                _materialService.DeleteFinishPriceHistoryFromMaterial(reference, finishReference,
                    enumerablePriceHistoryDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.DeleteBadRequest, "Deleting Failed: {0}",
                        ((ValidationOutputBadRequest) validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.DeleteNotFound, "Deleting Failed: {0}",
                        ((ValidationOutputNotFound) validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.DeleteInternalError,
                    "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.DeleteNoContent,
                    "Deleting Price History Of Finish {0} of Material By Reference: {1}", finishReference, reference,
                    EnumerableUtils.convert(enumerablePriceHistoryDto));
                _logger.logInformation(LoggingEvents.HardDeleteOk,
                    "Deleting Price History Of Finish {0} of Material By Reference: {1}", finishReference, reference,
                    EnumerableUtils.convert(enumerablePriceHistoryDto));
                return NoContent();
            }
        }
    }
}