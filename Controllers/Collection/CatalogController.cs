using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MerryClosets.Models;
using MerryClosets.Models.Collection;
using MerryClosets.Models.ConfiguredProduct;
using MerryClosets.Models.DTO;
using MerryClosets.Repositories;
using MerryClosets.Repositories.EF;
using MerryClosets.Services.EF;
using MerryClosets.Services.Interfaces;
using MerryClosets.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MerryClosets.Controllers
{
    [Route("api/catalog")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private ICatalogService _catalogService;
        private ICollectionService _collectionService;
        private IConfiguredProductService _configuredProductService;
        private readonly CustomLogger _logger;
        private readonly IUserValidationService _userValidationService;

        public CatalogController(IUserValidationService userValidationService, ICatalogService catalogService, ILogger<ICategoryService> logger, IConfiguredProductService configuredProductService, ICollectionService collectionService)
        {
            _catalogService = catalogService;
            _collectionService = collectionService;
            _configuredProductService = configuredProductService;
            _logger = new CustomLogger(logger);
            _userValidationService = userValidationService;
        }

        // ========= POST METHODS =========

        /**
         * POST method that will create a new Catalog in the system.
         */
        [HttpPost]
        public async Task<IActionResult> CreateCatalog([FromHeader(Name="Authorization")] string authorization, [FromBody] CatalogDto catalogDto)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }
            
            _logger.logInformation(LoggingEvents.PostItem, "Creating By Dto: {0}", catalogDto.Reference);
            ValidationOutput validationOutput = _catalogService.Register(catalogDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.PostBadRequest, "Creating Catalog Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.PostNotFound, "Creating Catalog Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                CatalogDto newCatalogDto = (CatalogDto)validationOutput.DesiredReturn;
                _logger.logInformation(LoggingEvents.PostOk, "Creating Catalog Succeeded: {0}",newCatalogDto.ToString());
                return CreatedAtRoute("GetCatalog", new { reference = newCatalogDto.Reference }, newCatalogDto);
            }
        }

        /**
         * POST method that allows the addition of new Configured Products to a Catalog (each Configured Product is associated with a Collection in the form of an object of type ProductCollection)
         */
        [HttpPost("{reference}/various-product-collection")]
        public async Task<IActionResult> AddVariousProductCollection([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference,
            [FromBody] IEnumerable<ProductCollectionDto> enumerableProductCollectionDto)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }
            
            object[] array = new object[2];
            array[0] = reference;
            array[1] = EnumerableUtils.convert(enumerableProductCollectionDto);
            _logger.logInformation(LoggingEvents.PostItem, "Adding Product Collections By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _catalogService.AddVariousProductCollection(reference, enumerableProductCollectionDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.PostBadRequest, "Adding Product Collections Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.PostNotFound, "Adding Product Collections Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.PostOk, "Adding Product Collections to Catalog Succeeded: {0}", array[1]);
                return Ok(validationOutput.DesiredReturn);
            }
        }

        // ========= GET METHODS =========

        /**
         * GET method that will return all existent catalogs in the system.
         */
        [HttpGet]
        public IActionResult GetAllCatalog()
        {
            IEnumerable<CatalogDto> list = _catalogService.GetAll();
            _logger.logInformation(LoggingEvents.GetAllOk, "Getting All Catalogs: {0}", EnumerableUtils.convert(list));
            return Ok(list);
        }

        /**
         * GET method that will return the catalog with the given reference.
         */
        [HttpGet("{reference}", Name = "GetCatalog")]
        public IActionResult GetCatalog([FromRoute] string reference)
        {
            _logger.logInformation(LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
            ValidationOutput validationOutput = _catalogService.GetByReference(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.GetItemBadRequest, "Getting Catalog Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.GetItemNotFound, "Getting Catalog Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.GetItemOk, "Getting Catalog: {0}", ((CatalogDto) validationOutput.DesiredReturn).ToString());
                return Ok((CatalogDto)validationOutput.DesiredReturn);
            }
        }

        // ========= PUT METHODS =========

        /**
         * PUT method that will update the name and the description of the catalog with the passed reference.
         */
        [HttpPut("{reference}")]
        public async Task<IActionResult> UpdateCatalog([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference, [FromBody] CatalogDto catalogDto)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }
            
            _logger.logInformation(LoggingEvents.UpdateItem, "Updating By Reference: {0}", reference);
            ValidationOutput validationOutput = _catalogService.Update(reference, catalogDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.UpdateBadRequest, "Updating Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.UpdateNotFound, "Updating Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.UpdateInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.UpdateNoContent, "Updating Catalog: {0}", ((CatalogDto)validationOutput.DesiredReturn).ToString());
                _logger.logInformation(LoggingEvents.UpdateOk, "Updating Catalog: {0}", ((CatalogDto)validationOutput.DesiredReturn).ToString());
                return NoContent();
            }
        }

        // ========= DELETE METHODS =========

        /**
         * DELETE method that will delete the catalog with the passed reference.
         */
        [HttpDelete("{reference}")]
        public async Task<IActionResult> DeleteCatalog([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }
            
            _logger.logInformation(LoggingEvents.SoftDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput = _catalogService.Remove(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.DeleteBadRequest, "Deleting Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.DeleteNotFound, "Deleting Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.DeleteInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.DeleteNoContent, "Deleting Catalog: {0}", reference);
                _logger.logInformation(LoggingEvents.SoftDeleteOk, "Deleting Catalog: {0}", reference);
                return NoContent();
            }
        }

        /**
         * DELETE method that allows the removal of Configured Products from a Catalog (each Configured Product is associated with a Collection in the form of an object of type ProductCollection)
         */
        [HttpDelete("{reference}/various-product-collection")]
        public IActionResult DeleteVariousProductCollection([FromRoute] string reference, [FromBody] IEnumerable<ProductCollectionDto> enumerableProductCollectionDto)
        {
            object[] array = new object[2];
            array[0] = reference;
            array[1] = EnumerableUtils.convert(enumerableProductCollectionDto);
            _logger.logInformation(LoggingEvents.HardDeleteItem, "Deleting Product Collections By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _catalogService.DeleteVariousProductCollection(reference, enumerableProductCollectionDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.DeleteBadRequest, "Deleting Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.DeleteNotFound, "Deleting Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.DeleteInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.DeleteNoContent, "Deleting Product Collections of Catalog: {0}", array[1]);
                _logger.logInformation(LoggingEvents.HardDeleteOk, "Deleting Product Collections of Catalog: {0}", array[1]);
                return NoContent();
            }
        }
    }
}