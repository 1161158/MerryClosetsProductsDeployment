using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MerryClosets.Models;
using MerryClosets.Models.DTO;
using MerryClosets.Models.Restriction;
using MerryClosets.Services.EF;
using MerryClosets.Services.Interfaces;
using MerryClosets.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace MerryClosets.Controllers.Product
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMaterialService _materialService;
        private readonly ICategoryService _categoryService;
        private readonly CustomLogger _logger;

        public ProductController(IProductService productService, ILogger<ICategoryService> logger, IMaterialService materialService, ICategoryService categoryService)
        {
            _productService = productService;
            _materialService = materialService;
            _categoryService = categoryService;
            _logger = new CustomLogger(logger);

        }

        // ========= POST METHODS =========

        /**
         * POST method that will create a new product in the system.
         */
        [HttpPost]
        public IActionResult CreateProduct([FromBody] ProductDto productDto)
        {
            _logger.logInformation(LoggingEvents.PostItem, "Creating By Dto: {0}", productDto.Reference);
            ValidationOutput validationOutput = _productService.Register(productDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.PostBadRequest, "Creating Product Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.PostNotFound, "Creating Product Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                ProductDto newProductDto = (ProductDto)validationOutput.DesiredReturn;
                _logger.logInformation(LoggingEvents.PostOk, "Creating Product Succeeded: {0}",newProductDto.ToString());
                return CreatedAtRoute("GetProduct", new { reference = newProductDto.Reference }, newProductDto);
            }
        }

        /**
         * POST method that allows the addition of materials in which a product can be built, to the product with the passed reference.
         */
        [HttpPost("{reference}/materials")]
        public IActionResult AddMaterials([FromRoute] string reference, [FromBody]  IEnumerable<ProductMaterialDto> enumerableProductMaterialDto)
        {
            object[] array = new object[2];
            array[0] = reference;
            array[1] = EnumerableUtils.convert(enumerableProductMaterialDto);
            _logger.logInformation(LoggingEvents.PostItem, "Adding Materials By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _productService.AddMaterialsAndRespectiveAlgorithms(reference, enumerableProductMaterialDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.PostBadRequest, "Adding Materials Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.PostNotFound, "Adding Materials Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.PostOk, "Adding Materials to Product Succeeded: {0}", array[1]);
                return Ok(validationOutput.DesiredReturn);
                
            }
        }

        /**
         * POST method that allows the addition of products which a product can be complemented with, to the product with the passed reference.
         */
        [HttpPost("{reference}/parts")]
        public IActionResult AddParts([FromRoute] string reference, [FromBody] IEnumerable<PartDto> enumerablePartDto)
        {
            object[] array = new object[2];
            array[0] = reference;
            array[1] = EnumerableUtils.convert(enumerablePartDto);
            _logger.logInformation(LoggingEvents.PostItem, "Adding Parts By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _productService.AddProductsAndRespectiveAlgorithms(reference, enumerablePartDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.PostBadRequest, "Adding Parts Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.PostNotFound, "Adding Parts Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.PostOk, "Adding Parts to Product Succeeded: {0}", array[1]);
                return Ok(validationOutput.DesiredReturn);
                
            }
        }

        /**
         * POST method that allows the addition of dimension values to the product with the passed reference.
         */
        [HttpPost("{reference}/dimension-values")]
        public IActionResult AddDimensionValues([FromRoute] string reference, [FromBody] IEnumerable<DimensionValuesDto> dimensionValuesDto)
        {
            object[] array = new object[2];
            
            array[0] = reference;
            List<DimensionValuesDto> dtos = dimensionValuesDto.ToList();
            
            var sb = new StringBuilder();
            
            for (var j = 0; j < dtos.Count; j++)
            {
                sb.Append("{Possible Heights: ");
                sb.Append(EnumerableUtils.convert(dtos[j].PossibleHeights));
                sb.Append(". Possible Widths: ");
                sb.Append(EnumerableUtils.convert(dtos[j].PossibleWidths));
                sb.Append(". Possible Depths: ");
                sb.Append(EnumerableUtils.convert(dtos[j].PossibleDepths));
                sb.Append("}");
            }

            array[1] = sb.ToString();
            _logger.logInformation(LoggingEvents.PostItem, "Adding Dimension Values By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _productService.AddVariousDimensionValues(reference, dimensionValuesDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.PostBadRequest, "Adding Dimension Values Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.PostNotFound, "Adding Dimension Values Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.PostOk, "Adding Dimension Values to Product Succeeded: {0}", sb.ToString());
                return Ok(validationOutput.DesiredReturn);
            }
        }

        [HttpPost("{reference}/restriction")]
        public IActionResult AddRestriction([FromRoute] string reference, [FromBody] AlgorithmDto restriction){
            object[] array = new object[2];
            array[0] = reference;
            array[1] = restriction.ToString();
            _logger.logInformation(LoggingEvents.PostItem, "Adding Restriction By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _productService.AddRestriction(reference, restriction);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.PostBadRequest, "Adding Restriction Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.PostNotFound, "Adding Restriction Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.PostOk, "Adding Restriction to Product Succeeded: {0}", "");
                return Ok(validationOutput.DesiredReturn);            
            }
        }

        // ========= GET METHODS =========

        /**
         * GET method that will return all existent products in the system.
         */
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            IEnumerable<ProductDto> list = _productService.GetAll();
            _logger.logInformation(LoggingEvents.GetAllOk, "Getting All Products: {0}", EnumerableUtils.convert(list));
            return Ok(list);           
        }

        /**
         * GET method that will return the product with the given reference.
         */
        [HttpGet("{reference}", Name = "GetProduct")]
        public IActionResult GetProduct([FromRoute] string reference)
        {
            _logger.logInformation(LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
            ValidationOutput validationOutput = _productService.GetByReference(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.GetItemBadRequest, "Getting Product Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.GetItemNotFound, "Getting Product Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.GetItemOk, "Getting Product: {0}", ((ProductDto) validationOutput.DesiredReturn).ToString());
                return Ok((ProductDto)validationOutput.DesiredReturn);
            }
        }

        /**
         * GET method that will return all the materials that were associated with the product with the passed reference.
         */
        [HttpGet("{reference}/materials")]
        public IActionResult GetMaterialsAvailableToProduct([FromRoute] string reference)
        {
            _logger.logInformation(LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
            ValidationOutput validationOutput = _productService.GetMaterialsAvailableToProduct(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.GetItemBadRequest, "Getting Materials Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.GetItemNotFound, "Getting Materials Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.GetItemOk, "Getting Materials: {0}",
                    EnumerableUtils.convert((List<MaterialDto>) validationOutput.DesiredReturn));
                return Ok((List<MaterialDto>)validationOutput.DesiredReturn);
            }
        }

        /**
         * GET method that will return all the products that can complement the product with the passed reference.
         */
        [HttpGet("{reference}/parts")]
        public IActionResult GetPartProductsAvailableToProduct([FromRoute] string reference)
        {
            _logger.logInformation(LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
            ValidationOutput validationOutput = _productService.GetPartProductsAvailableToProduct(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(LoggingEvents.GetItemBadRequest, "Getting Parts Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(LoggingEvents.GetItemNotFound, "Getting Parts Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(LoggingEvents.GetItemOk, "Getting Parts: {0}", EnumerableUtils.convert((List<ProductDto>)validationOutput.DesiredReturn));
                return Ok((List<ProductDto>)validationOutput.DesiredReturn);
            }
        }

        // ========= PUT METHODS =========

        /**
         * PUT method that will update the name, the description and the price of the product with the passed reference.
         */
        [HttpPut("{reference}")]
        public IActionResult UpdateProduct([FromRoute] string reference, [FromBody] ProductDto productDto)
        {
            _logger.logInformation(LoggingEvents.UpdateItem, "Updating By Reference: {0}", reference);
            ValidationOutput validationOutput = _productService.Update(reference, productDto);
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
                _logger.logInformation(LoggingEvents.UpdateNoContent, "Updating Product: {0}", ((ProductDto)validationOutput.DesiredReturn).ToString());
                _logger.logInformation(LoggingEvents.UpdateOk, "Updating Product: {0}", ((ProductDto)validationOutput.DesiredReturn).ToString());
                return NoContent();
            }
        }

        // ========= DELETE METHODS =========

        /**
         * DELETE method that will soft-delete the product with the passed reference.
         */
        [HttpDelete("{reference}")]
        public IActionResult DeleteProduct([FromRoute] string reference)
        {
            _logger.logInformation(LoggingEvents.SoftDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput = _productService.Remove(reference);
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
                _logger.logInformation(LoggingEvents.DeleteNoContent, "Deleting Product: {0}", reference);
                _logger.logInformation(LoggingEvents.SoftDeleteOk, "Deleting Product: {0}", reference);
                return NoContent();
            }
        }

        /*
         * DELETE method that will delete the list of passed materials from the product with the passed reference.
         */
        [HttpDelete("{reference}/materials")]
        public IActionResult DeleteMaterials([FromRoute] string reference, [FromBody]  IEnumerable<MaterialDto> enumerableMaterial)
        {
            _logger.logInformation(LoggingEvents.HardDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput = _productService.DeleteMaterialsAndRespectiveAlgorithms(reference, enumerableMaterial);
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
                _logger.logInformation(LoggingEvents.DeleteNoContent, "Deleting Materials From Product: {0}", EnumerableUtils.convert(enumerableMaterial));
                _logger.logInformation(LoggingEvents.HardDeleteOk, "Deleting Materials From Product: {0}", EnumerableUtils.convert(enumerableMaterial));
                return NoContent();
            }
        }

        /*
         * DELETE method that will delete the list of passed parts from the product with the passed reference.
         */
        [HttpDelete("{reference}/parts")]
        public IActionResult DeleteParts([FromRoute] string reference, [FromBody] IEnumerable<ProductDto> enumerableProductReference)
        {
            _logger.logInformation(LoggingEvents.HardDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput = _productService.DeleteProductsAndRespectiveAlgorithms(reference, enumerableProductReference);
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
                _logger.logInformation(LoggingEvents.DeleteNoContent, "Deleting Parts From Product: {0}", EnumerableUtils.convert(enumerableProductReference));
                _logger.logInformation(LoggingEvents.HardDeleteOk, "Deleting Parts From Product: {0}", EnumerableUtils.convert(enumerableProductReference));
                return NoContent();
            }
        }

        /*
         * DELETE method that will delete the list of dimension values from the product with the passed reference.
         */
        [HttpDelete("{reference}/dimension-values")]
        public IActionResult DeleteDimensionValues([FromRoute] string reference, [FromBody] IEnumerable<DimensionValuesDto> enumerableDimensionValuesDto)
        {
            _logger.logInformation(LoggingEvents.HardDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput = _productService.DeleteVariousDimensionValues(reference, enumerableDimensionValuesDto);
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
                _logger.logInformation(LoggingEvents.DeleteNoContent, "Deleting Dimension Values From Product: {0}", EnumerableUtils.convert(enumerableDimensionValuesDto));
                _logger.logInformation(LoggingEvents.HardDeleteOk, "Deleting Dimension Values From Product: {0}", EnumerableUtils.convert(enumerableDimensionValuesDto));
                return NoContent();
            }
        }
    }
}