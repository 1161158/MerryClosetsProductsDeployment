using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MerryClosets.Models;
using MerryClosets.Models.ConfiguredProduct;
using MerryClosets.Models.DTO;
using MerryClosets.Models.Product;
using MerryClosets.Repositories;
using MerryClosets.Repositories.EF;
using MerryClosets.Services.Interfaces;
using MerryClosets.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MerryClosets.Controllers
{

    [Route("api/configuredProduct")]
    [ApiController]
    public class ConfiguredProductController : ControllerBase
    {
        private readonly IConfiguredProductService _configuredProductService;

        private readonly IProductService _productService;

        private readonly CustomLogger _logger;
        private readonly IMaterialService _materialService;

        private readonly IUserValidationService _userValidationService;
        
        public ConfiguredProductController(IUserValidationService userValidationService, IConfiguredProductService configuredProductService, ILogger<ICategoryService> logger, IProductService productService, IMaterialService materialService)
        {
            _configuredProductService = configuredProductService;
            _productService = productService;
            _materialService = materialService;
            _logger = new CustomLogger(logger);
            _userValidationService = userValidationService;
        }

        // ========= POST METHODS =========

        /**
         * POST method that will create a new configured product in the system.
         */
        [HttpPost]
        public async Task<IActionResult> Create([FromHeader(Name="Authorization")] string authorization, ChildConfiguredProductDto receivedDto)
        {
            if(!_userValidationService.CheckAuthorizationToken(authorization)) {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            
            _logger.logInformation(userRef, LoggingEvents.PostItem, "Creating By Dto: {0}", receivedDto.Reference);
            ValidationOutput validationOutput = _configuredProductService.Register(receivedDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostBadRequest, "Creating Configured Product Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostNotFound, "Creating Configured Product Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                ConfiguredProductDto dto = (ConfiguredProductDto)validationOutput.DesiredReturn;
                _logger.logInformation(userRef, LoggingEvents.PostOk, "Creating Configured Product Succeeded: {0}",dto.ToString());
                return CreatedAtRoute("GetConfiguredProduct", new { reference = receivedDto.Reference }, dto);
            }
        }

        // ========= GET METHODS =========

        /**
         * GET method that will return all existent configured products in the system.
         */
        [HttpGet]
        public async Task<IActionResult> GetAllConfiguredProducts([FromHeader(Name="Authorization")] string authorization)
        {
            if(!_userValidationService.CheckAuthorizationToken(authorization)) {
                return Unauthorized();
            }
            if(!(await _userValidationService.Validate(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }
            
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            
            IEnumerable<ConfiguredProductDto> list = _configuredProductService.GetAll();
            _logger.logInformation(userRef, LoggingEvents.GetAllOk, "Getting All Configured Products: {0}", EnumerableUtils.convert(list));
            return Ok(list);
        }

        /**
         * GET method that will return the category with the given reference.
         */
        [HttpGet("{reference}", Name = "GetConfiguredProduct")]
        public async Task<ActionResult> GetByReference([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference)
        {
            if(!_userValidationService.CheckAuthorizationToken(authorization)) {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            ValidationOutput validationOutput;
            
            if(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])) {
                _logger.logInformation(userRef, LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
                validationOutput = _configuredProductService.GetByReference(reference);
            }
            else if(await _userValidationService.Validate(authorization.Split(" ")[1]))
            {
                _logger.logInformation(userRef, LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
                validationOutput = _configuredProductService.ClientGetByReference(reference);
            }else
            {
                return Unauthorized();
            }
            
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemBadRequest, "Getting Configured Product Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemNotFound, "Getting Configured Product Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.GetItemOk, "Getting Configured Product: {0}", ((ConfiguredProductDto) validationOutput.DesiredReturn).ToString());
                return Ok((ConfiguredProductDto)validationOutput.DesiredReturn);
            }
        }

        /**
         * GET method that will return all the information necessary, of the configured product with the passed reference.
         * Instead of bringing references, it brings the actual object.
         */
        [HttpGet("{reference}/all-info")]
        public async Task<ActionResult> GetAllInfoByReference([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference)
        {
            if(!_userValidationService.CheckAuthorizationToken(authorization)) {
                return Unauthorized();
            }
            var userRef = "";
            if (authorization != null)
            {
                userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            }
            
            _logger.logInformation(userRef, LoggingEvents.GetItem, "Getting Info By Reference: {0}", reference);
            ValidationOutput validationOutput = _configuredProductService.GetAllInfoByReference(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemBadRequest, "Getting Info Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemNotFound, "Getting Info Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please, contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.GetItemOk, "Getting Info: {0}", ((ConfiguredProductDto)validationOutput.DesiredReturn).ToString());
                return Ok((ProductOrderDto)validationOutput.DesiredReturn);
            }
        }

        [HttpGet("{reference}/available-products")]
        public async Task<ActionResult> GetAvailableProducts([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference)
        {
            if(!_userValidationService.CheckAuthorizationToken(authorization)) {
                return Unauthorized();
            }
            if(!(await _userValidationService.Validate(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }
            
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            
            _logger.logInformation(userRef, LoggingEvents.GetItem, "Getting Available Products By Reference: {0}", reference);
            ValidationOutput validationOutput = _configuredProductService.GetAvailableProducts(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemBadRequest, "Getting Available Products Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemNotFound, "Getting Available Products Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please, contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.GetItemOk, "Getting Available Products: {0}", reference);
                return Ok((List<ProductDto>)validationOutput.DesiredReturn);
            }
        }

        // ========= PUT METHODS =========

        // ========= DELETE METHODS =========

        [HttpDelete("{reference}")]
        public async Task<IActionResult> DeleteProduct([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference)
        {
            if(!_userValidationService.CheckAuthorizationToken(authorization)) {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            
            _logger.logInformation(userRef, LoggingEvents.SoftDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput = _configuredProductService.Remove(reference);
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
                _logger.logInformation(userRef, LoggingEvents.DeleteNoContent, "Deleting Configured Product: {0}", reference);
                _logger.logInformation(userRef, LoggingEvents.SoftDeleteOk, "Deleting Configured Product: {0}", reference);
                return NoContent();
            }
        }
    }
}