using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MerryClosets.Models.DTO;
using MerryClosets.Services.EF;
using MerryClosets.Services.Interfaces;
using MerryClosets.Utils;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace MerryClosets.Controllers.Category
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly CustomLogger _logger;
        private readonly IUserValidationService _userValidationService;
        public CategoryController(ICategoryService categoryService, IProductService productService, IUserValidationService userValidationService, ILogger<ICategoryService> logger)
        {
            _categoryService = categoryService;
            _productService = productService;
            _userValidationService = userValidationService;
            _logger = new CustomLogger(logger);
        }

        // ========= POST METHODS =========

        /**
         * POST method that will create a new category in the system.
         */
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromHeader(Name="Authorization")] string authorization, [FromBody] CategoryDto categoryDto)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }

            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            _logger.logInformation(userRef, LoggingEvents.PostItem, "Creating By Dto: {0}", categoryDto.Reference);

            ValidationOutput validationOutput = _categoryService.Register(categoryDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostBadRequest, "Creating Category Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostNotFound, "Creating Category Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                CategoryDto newCategoryDto = (CategoryDto)validationOutput.DesiredReturn;
                _logger.logInformation(userRef, LoggingEvents.PostOk, "Creating Category Succeeded: {0}", newCategoryDto.ToString());
                return CreatedAtRoute("GetCategory", new { reference = newCategoryDto.Reference }, newCategoryDto);
            }
        }

        // ========= GET METHODS =========

        /**
         * GET method that will return all existent categories in the system.
         */
        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromHeader(Name="Authorization")] string authorization)
        {
            var userRef = "";
            if (authorization != null)
            {
                userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            }
            
            IEnumerable<CategoryDto> list = _categoryService.GetAll();
            _logger.logInformation(userRef, LoggingEvents.GetAllOk, "Getting All Categories: {0}", EnumerableUtils.convert(list));
            return Ok(list);
        }

        /**
         * GET method that will return the category with the given reference.
         */
        [HttpGet("{reference}", Name = "GetCategory")]
        public async Task<IActionResult> GetCategory([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference)
        {
            var userRef = "";
            if (authorization != null)
            {
                userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            }
            
            _logger.logInformation(userRef, LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
            ValidationOutput validationOutput = _categoryService.GetByReference(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemBadRequest, "Getting Category Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemNotFound, "Getting Category Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }
                
                _logger.logCritical(userRef, LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.GetItemOk, "Getting Category: {0}", ((CategoryDto) validationOutput.DesiredReturn).ToString());
                return Ok((CategoryDto)validationOutput.DesiredReturn);
            }
        }

        /**
         * GET method that will return the children of the category with the given reference.
         */
        [HttpGet("{reference}/direct-child-categories", Name = "GetDirectChildCategories")]
        public async Task<IActionResult> GetDirectChildCategories([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference)
        {
            var userRef = "";
            if (authorization != null)
            {
                userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            }
            
            _logger.logInformation(userRef, LoggingEvents.GetAllItems, "Getting Children By Reference: {0}", reference);
            ValidationOutput validationOutput = _categoryService.ObtainDirectChildCategories(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetAllBadRequest, "Getting Children Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetAllNotFound, "Getting Children Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.GetAllInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.GetAllOk, "Getting Children: {0}", EnumerableUtils.convert((List<CategoryDto>)validationOutput.DesiredReturn));
                return Ok(validationOutput.DesiredReturn);
            }
        }

        /**
         * GET method that will return all products whose category they belong is the one with the given reference.
         */
        [HttpGet("{reference}/products")]
        public async Task<IActionResult> GetProductsOfCategory([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference)
        {
            var userRef = "";
            if (authorization != null)
            {
                userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            }
            
            _logger.logInformation(userRef, LoggingEvents.GetAllItems, "Getting Products By Reference: {0}", reference);
            ValidationOutput validationOutput = _productService.GetProductsOfCategory(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetAllBadRequest, "Getting Products Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetAllNotFound, "Getting Products Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.GetAllInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.GetAllOk, "Getting Products: {0}", EnumerableUtils.convert((List<ProductDto>)validationOutput.DesiredReturn));
                return Ok(validationOutput.DesiredReturn);
            }
        }

        // ========= PUT METHODS =========

        /**
         * PUT method that will update the name and the description of the category with the passed reference.
         */
        [HttpPut("{reference}")]
        public async Task<IActionResult> UpdateCategory([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference, [FromBody] CategoryDto categoryDto)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }
            
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            
            _logger.logInformation(userRef, LoggingEvents.UpdateItem, "Updating By Reference: {0}", reference);
            ValidationOutput validationOutput = _categoryService.Update(reference, categoryDto);
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
                _logger.logInformation(userRef, LoggingEvents.UpdateNoContent, "Updating Category: {0}", ((CategoryDto)validationOutput.DesiredReturn).ToString());
                _logger.logInformation(userRef, LoggingEvents.UpdateOk, "Updating Category: {0}", ((CategoryDto)validationOutput.DesiredReturn).ToString());
                return NoContent();
            }
        }

        // ========= DELETE METHODS =========

        /**
         * DELETE method that will delete the category with the passed reference.
         */
        [HttpDelete("{reference}")]
        public async Task<IActionResult> DeleteCategory([FromHeader(Name="Authorization")] string authorization, [FromRoute] string reference)
        {
            if(!(await _userValidationService.validateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }
            
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            
            _logger.logInformation(userRef, LoggingEvents.SoftDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput = _categoryService.Remove(reference);
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
                _logger.logInformation(userRef, LoggingEvents.DeleteNoContent, "Deleting Category: {0}", reference);
                _logger.logInformation(userRef, LoggingEvents.SoftDeleteOk, "Deleting Category: {0}", reference);
                return NoContent();
            }
        }
    }
}