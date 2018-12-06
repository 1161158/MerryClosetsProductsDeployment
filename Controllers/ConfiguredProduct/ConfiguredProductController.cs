using System;
using System.Collections.Generic;
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

        public ConfiguredProductController(IConfiguredProductService configuredProductService, ILogger<ICategoryService> logger, IProductService productService, IMaterialService materialService)
        {
            _configuredProductService = configuredProductService;
            _productService = productService;
            _materialService = materialService;
            _logger = new CustomLogger(logger);
        }

        // ========= POST METHODS =========

        /**
         * POST method that will create a new configured product in the system.
         */
        [HttpPost]
        public IActionResult Create(ChildConfiguredProductDto receivedDto)
        {
            _logger.logInformation(LoggingEvents.PostItem, "Creating By Dto: {0}", receivedDto.Reference);
            ValidationOutput validationOutput = _configuredProductService.Register(receivedDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.PostBadRequest, "Creating Configured Product Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.PostNotFound, "Creating Configured Product Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                ConfiguredProductDto dto = (ConfiguredProductDto)validationOutput.DesiredReturn;
                _logger.logInformation(LoggingEvents.PostOk, "Creating Configured Product Succeeded: {0}",dto.ToString());
                return CreatedAtRoute("GetConfiguredProduct", new { reference = receivedDto.Reference }, dto);
            }
        }

        // ========= GET METHODS =========

        /**
         * GET method that will return all existent configured products in the system.
         */
        [HttpGet]
        public IActionResult GetAllConfiguredProducts()
        {
            IEnumerable<ConfiguredProductDto> list = _configuredProductService.GetAll();
            _logger.logInformation(LoggingEvents.GetAllOk, "Getting All Configured Products: {0}", EnumerableUtils.convert(list));
            return Ok(list);
        }

        /**
         * GET method that will return the category with the given reference.
         */
        [HttpGet("{reference}", Name = "GetConfiguredProduct")]
        public ActionResult GetByReference([FromRoute] string reference)
        {
            _logger.logInformation(LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
            ValidationOutput validationOutput = _configuredProductService.GetByReference(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.GetItemBadRequest, "Getting Configured Product Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.GetItemNotFound, "Getting Configured Product Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.GetItemOk, "Getting Configured Product: {0}", ((ConfiguredProductDto) validationOutput.DesiredReturn).ToString());
                return Ok((ConfiguredProductDto)validationOutput.DesiredReturn);
            }
        }

        /**
         * GET method that will return all the information necessary, of the configured product with the passed reference.
         * Instead of bringing references, it brings the actual object.
         */
        [HttpGet("{reference}/all-info")]
        public ActionResult GetAllInfoByReference([FromRoute] string reference)
        {
            _logger.logInformation(LoggingEvents.GetItem, "Getting Info By Reference: {0}", reference);
            ValidationOutput validationOutput = _configuredProductService.GetAllInfoByReference(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.GetItemBadRequest, "Getting Info Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.GetItemNotFound, "Getting Info Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please, contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.GetItemOk, "Getting Info: {0}", ((ConfiguredProductDto)validationOutput.DesiredReturn).ToString());
                return Ok((ProductOrderDto)validationOutput.DesiredReturn);
            }
        }

        [HttpGet("{reference}/available-products")]
        public ActionResult GetAvailableProducts([FromRoute] string reference)
        {
            _logger.logInformation(LoggingEvents.GetItem, "Getting Available Products By Reference: {0}", reference);
            ValidationOutput validationOutput = _configuredProductService.GetAvailableProducts(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.GetItemBadRequest, "Getting Available Products Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.GetItemNotFound, "Getting Available Products Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please, contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.GetItemOk, "Getting Available Products: {0}", reference);
                return Ok((List<ProductDto>)validationOutput.DesiredReturn);
            }
        }

        // ========= PUT METHODS =========

        // ========= DELETE METHODS =========

        [HttpDelete("{reference}")]
        public IActionResult DeleteProduct([FromRoute] string reference)
        {
            _logger.logInformation(LoggingEvents.SoftDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput = _configuredProductService.Remove(reference);
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
                _logger.logInformation(LoggingEvents.DeleteNoContent, "Deleting Configured Product: {0}", reference);
                _logger.logInformation(LoggingEvents.SoftDeleteOk, "Deleting Configured Product: {0}", reference);
                return NoContent();
            }
        }
    }
}