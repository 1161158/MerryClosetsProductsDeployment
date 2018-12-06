using System.Collections.Generic;
using MerryClosets.Models.DTO;
using MerryClosets.Services.Interfaces;
using MerryClosets.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MerryClosets.Controllers
{
    [Route("api/collection")]
    [ApiController]
    public class CollectionController : ControllerBase
    {
        private ICollectionService _collectionService;
        private IConfiguredProductService _configuredProductService;
        private readonly CustomLogger _logger;

        public CollectionController(ICollectionService service, ILogger<ICategoryService> logger, IConfiguredProductService configuredProductService)
        {
            _collectionService = service;
            _configuredProductService = configuredProductService;
            _logger = new CustomLogger(logger);
        }

        // ========= POST METHODS =========

        /**
         * POST method that will create a new collection in the system.
         */
        [HttpPost]
        public IActionResult CreateCollection(CollectionDto collectionDto)
        {
            _logger.logInformation(LoggingEvents.PostItem, "Creating By Dto: {0}", collectionDto.Reference);
            ValidationOutput validationOutput = _collectionService.Register(collectionDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.PostBadRequest, "Creating Collection Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.PostNotFound, "Creating Collection Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                CollectionDto newCollectionDto = (CollectionDto)validationOutput.DesiredReturn;
                _logger.logInformation(LoggingEvents.PostOk, "Creating Collection Succeeded: {0}",newCollectionDto.ToString());
                return CreatedAtRoute("GetCollection", new { reference = newCollectionDto.Reference }, newCollectionDto);
            }
        }

        /**
         * POST method that will add configured products to the collection with the passed reference.
         */
        [HttpPost("{reference}/configured-products")]
        public IActionResult AddConfiguredProducts([FromRoute] string reference, [FromBody] IEnumerable<ProductCollectionDto> enumerableConfiguredProductReference)
        {
            object[] array = new object[2];
            array[0] = reference;
            array[1] = EnumerableUtils.convert(enumerableConfiguredProductReference);
            _logger.logInformation(LoggingEvents.PostItem, "Adding Configured Products By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _collectionService.AddConfiguredProducts(reference, enumerableConfiguredProductReference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.PostBadRequest, "Adding Configured Products Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.PostNotFound, "Adding Configured Products Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.PostOk, "Adding Configured Products to Collection Succeeded: {0}", array[1]);
                return Ok(validationOutput.DesiredReturn);
            }
        }

        // ========= GET METHODS =========

        /**
         * GET method that will return all existent collections in the system.
         */
        [HttpGet]
        public IActionResult GetAllCollections()
        {
            IEnumerable<CollectionDto> list = _collectionService.GetAll();
            _logger.logInformation(LoggingEvents.GetAllOk, "Getting All Collections: {0}", EnumerableUtils.convert(list));
            return Ok(list);
        }

        /**
         * GET method that will return the collection with the given reference.
         */
        [HttpGet("{reference}", Name = "GetCollection")]
        public IActionResult GetCollection([FromRoute] string reference)
        {
            _logger.logInformation(LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
            ValidationOutput validationOutput = _collectionService.GetByReference(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.GetItemBadRequest, "Getting Collection Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.GetItemNotFound, "Getting Collection Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.GetItemOk, "Getting Collection: {0}", ((CollectionDto) validationOutput.DesiredReturn).ToString());
                return Ok((CollectionDto)validationOutput.DesiredReturn);
            }
        }

        // ========= PUT METHODS =========

        /**
         * PUT method that will update the name and the description of the collection with the passed reference.
         */
        [HttpPut("{reference}")]
        public IActionResult UpdateCollection([FromRoute] string reference, [FromBody] CollectionDto collectionDto)
        {
            _logger.logInformation(LoggingEvents.UpdateItem, "Updating By Reference: {0}", reference);
            ValidationOutput validationOutput = _collectionService.Update(reference, collectionDto);
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
                _logger.logInformation(LoggingEvents.UpdateNoContent, "Updating Collection: {0}", ((CollectionDto)validationOutput.DesiredReturn).ToString());
                _logger.logInformation(LoggingEvents.UpdateOk, "Updating Collection: {0}", ((CollectionDto)validationOutput.DesiredReturn).ToString());
                return NoContent();
            }
        }

        // ========= DELETE METHODS =========

        /**
         * DELETE method that will delete the category with the passed reference.
         */
        [HttpDelete("{reference}")]
        public IActionResult DeleteCollection([FromRoute] string reference)
        {
            _logger.logInformation(LoggingEvents.SoftDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput = _collectionService.Remove(reference);
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
                _logger.logInformation(LoggingEvents.DeleteNoContent, "Deleting Collection: {0}", reference);
                _logger.logInformation(LoggingEvents.SoftDeleteOk, "Deleting Collection: {0}", reference);
                return NoContent();
            }
        }

        /**
         * DELETE method that will delete configured products from the collection with the passed reference.
         */
        [HttpDelete("{collectionReference}/configured-products")]
        public IActionResult DeleteConfiguredProducts([FromRoute] string reference, [FromBody] IEnumerable<ProductCollectionDto> enumerableConfiguredProductReference)
        {
            object[] array = new object[2];
            array[0] = reference;
            array[1] = EnumerableUtils.convert(enumerableConfiguredProductReference);
            _logger.logInformation(LoggingEvents.HardDeleteItem, "Deleting Configured Products By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _collectionService.DeleteConfiguredProducts(reference, enumerableConfiguredProductReference);
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
                _logger.logInformation(LoggingEvents.DeleteNoContent, "Deleting Configured Products of Collection: {0}", array[1]);
                _logger.logInformation(LoggingEvents.HardDeleteOk, "Deleting Configured Products of Collection: {0}", array[1]);
                return NoContent();
            }
        }
    }
}