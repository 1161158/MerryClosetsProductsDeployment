using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly IUserValidationService _userValidationService;

        public CollectionController(IUserValidationService userValidationService, ICollectionService service, ILogger<ICategoryService> logger, IConfiguredProductService configuredProductService)
        {
            _collectionService = service;
            _configuredProductService = configuredProductService;
            _logger = new CustomLogger(logger);
            _userValidationService = userValidationService;
        }

        // ========= POST METHODS =========

        /**
         * POST method that will create a new collection in the system.
         */
        [HttpPost]
        public async Task<IActionResult> CreateCollection([FromHeader(Name="Authorization")] string authorization, [FromBody] CollectionDto collectionDto)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }
            
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            
            _logger.logInformation(userRef, LoggingEvents.PostItem, "Creating By Dto: {0}", collectionDto.Reference);
            ValidationOutput validationOutput = _collectionService.Register(collectionDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostBadRequest, "Creating Collection Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostNotFound, "Creating Collection Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                CollectionDto newCollectionDto = (CollectionDto)validationOutput.DesiredReturn;
                _logger.logInformation(userRef, LoggingEvents.PostOk, "Creating Collection Succeeded: {0}",newCollectionDto.ToString());
                return CreatedAtRoute("GetCollection", new { reference = newCollectionDto.Reference }, newCollectionDto);
            }
        }

        /**
         * POST method that will add configured products to the collection with the passed reference.
         */
        [HttpPost("{reference}/configured-products")]
        public async Task<IActionResult> AddConfiguredProducts([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference, [FromBody] IEnumerable<ProductCollectionDto> enumerableConfiguredProductReference)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }
            
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            
            object[] array = new object[2];
            array[0] = reference;
            array[1] = EnumerableUtils.convert(enumerableConfiguredProductReference);
            _logger.logInformation(userRef, LoggingEvents.PostItem, "Adding Configured Products By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _collectionService.AddConfiguredProducts(reference, enumerableConfiguredProductReference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostBadRequest, "Adding Configured Products Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostNotFound, "Adding Configured Products Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.PostOk, "Adding Configured Products to Collection Succeeded: {0}", array[1]);
                return Ok(validationOutput.DesiredReturn);
            }
        }

        // ========= GET METHODS =========

        /**
         * GET method that will return all existent collections in the system.
         */
        [HttpGet]
        public async Task<IActionResult> GetAllCollections([FromHeader(Name="Authorization")] string authorization)
        {
            var userRef = "";
            if (authorization != null)
            {
                userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            }
            
            IEnumerable<CollectionDto> list = _collectionService.GetAll();
            _logger.logInformation(userRef, LoggingEvents.GetAllOk, "Getting All Collections: {0}", EnumerableUtils.convert(list));
            return Ok(list);
        }

        /**
         * GET method that will return the collection with the given reference.
         */
        [HttpGet("{reference}", Name = "GetCollection")]
        public async Task<IActionResult> GetCollection([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference)
        {
            var userRef = "";
            if (authorization != null)
            {
                userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            }
            
            _logger.logInformation(userRef, LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
            ValidationOutput validationOutput = _collectionService.GetByReference(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemBadRequest, "Getting Collection Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemNotFound, "Getting Collection Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.GetItemOk, "Getting Collection: {0}", ((CollectionDto) validationOutput.DesiredReturn).ToString());
                return Ok((CollectionDto)validationOutput.DesiredReturn);
            }
        }

        // ========= PUT METHODS =========

        /**
         * PUT method that will update the name and the description of the collection with the passed reference.
         */
        [HttpPut("{reference}")]
        public async Task<IActionResult> UpdateCollection([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference, [FromBody] CollectionDto collectionDto)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }
            
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            
            _logger.logInformation(userRef, LoggingEvents.UpdateItem, "Updating By Reference: {0}", reference);
            ValidationOutput validationOutput = _collectionService.Update(reference, collectionDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.UpdateBadRequest, "Updating Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.UpdateNotFound, "Updating Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.UpdateInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.UpdateNoContent, "Updating Collection: {0}", ((CollectionDto)validationOutput.DesiredReturn).ToString());
                _logger.logInformation(userRef, LoggingEvents.UpdateOk, "Updating Collection: {0}", ((CollectionDto)validationOutput.DesiredReturn).ToString());
                return NoContent();
            }
        }

        // ========= DELETE METHODS =========

        /**
         * DELETE method that will delete the category with the passed reference.
         */
        [HttpDelete("{reference}")]
        public async Task<IActionResult> DeleteCollection([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }
            
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            
            _logger.logInformation(userRef, LoggingEvents.SoftDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput = _collectionService.Remove(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.DeleteBadRequest, "Deleting Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.DeleteNotFound, "Deleting Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.DeleteInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.DeleteNoContent, "Deleting Collection: {0}", reference);
                _logger.logInformation(userRef, LoggingEvents.SoftDeleteOk, "Deleting Collection: {0}", reference);
                return NoContent();
            }
        }

        /**
         * DELETE method that will delete configured products from the collection with the passed reference.
         */
        [HttpDelete("{collectionReference}/configured-products")]
        public async Task<IActionResult> DeleteConfiguredProducts([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference, [FromBody] IEnumerable<ProductCollectionDto> enumerableConfiguredProductReference)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }
            
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            
            object[] array = new object[2];
            array[0] = reference;
            array[1] = EnumerableUtils.convert(enumerableConfiguredProductReference);
            _logger.logInformation(userRef, LoggingEvents.HardDeleteItem, "Deleting Configured Products By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _collectionService.DeleteConfiguredProducts(reference, enumerableConfiguredProductReference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.DeleteBadRequest, "Deleting Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.DeleteNotFound, "Deleting Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.DeleteInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.DeleteNoContent, "Deleting Configured Products of Collection: {0}", array[1]);
                _logger.logInformation(userRef, LoggingEvents.HardDeleteOk, "Deleting Configured Products of Collection: {0}", array[1]);
                return NoContent();
            }
        }
    }
}