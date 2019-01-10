using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly IUserValidationService _userValidationService;

        public ProductController(IUserValidationService userValidationService, IProductService productService, ILogger<ICategoryService> logger, IMaterialService materialService, ICategoryService categoryService)
        {
            _productService = productService;
            _materialService = materialService;
            _categoryService = categoryService;
            _logger = new CustomLogger(logger);
            _userValidationService = userValidationService;

        }

        // ========= POST METHODS =========

        /**
         * POST method that will create a new product in the system.
         */
        [HttpPost]
        public async Task<IActionResult> CreateProduct( [FromHeader(Name = "Authorization")] string authorization, [FromBody] ProductDto productDto)
        {
            if(!_userValidationService.CheckAuthorizationToken(authorization)) {
                return Unauthorized();
            }
            if(!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1]))) {
                return Unauthorized();
            }

            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            _logger.logInformation(userRef, LoggingEvents.PostItem, "Creating By Dto: {0}", productDto.Reference);
            ValidationOutput validationOutput = _productService.Register(productDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostBadRequest, "Creating Product Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostNotFound, "Creating Product Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                ProductDto newProductDto = (ProductDto)validationOutput.DesiredReturn;
                _logger.logInformation(userRef, LoggingEvents.PostOk, "Creating Product Succeeded: {0}", newProductDto.ToString());
                return CreatedAtRoute("GetProduct", new { reference = newProductDto.Reference }, newProductDto);
            }
        }

        /**
         * POST method that allows the addition of materials in which a product can be built, to the product with the passed reference.
         */
        [HttpPost("{reference}/materials")]
        public async Task<IActionResult> AddMaterials([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string reference, [FromBody]  IEnumerable<ProductMaterialDto> enumerableProductMaterialDto)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            object[] array = new object[2];
            array[0] = reference;
            array[1] = EnumerableUtils.convert(enumerableProductMaterialDto);
            _logger.logInformation(userRef, LoggingEvents.PostItem, "Adding Materials By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _productService.AddMaterialsAndRespectiveAlgorithms(reference, enumerableProductMaterialDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostBadRequest, "Adding Materials Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostNotFound, "Adding Materials Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.PostOk, "Adding Materials to Product Succeeded: {0}", array[1]);
                return Ok(validationOutput.DesiredReturn);

            }
        }

        /**
         * POST method that allows the addition of products which a product can be complemented with, to the product with the passed reference.
         */
        [HttpPost("{reference}/parts")]
        public async Task<IActionResult> AddParts([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string reference, [FromBody] IEnumerable<PartDto> enumerablePartDto)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            object[] array = new object[2];
            array[0] = reference;
            array[1] = EnumerableUtils.convert(enumerablePartDto);
            _logger.logInformation(userRef, LoggingEvents.PostItem, "Adding Parts By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _productService.AddProductsAndRespectiveAlgorithms(reference, enumerablePartDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostBadRequest, "Adding Parts Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostNotFound, "Adding Parts Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.PostOk, "Adding Parts to Product Succeeded: {0}", array[1]);
                return Ok(validationOutput.DesiredReturn);

            }
        }

        /**
         * POST method that allows the addition of dimension values to the product with the passed reference.
         */
        // [HttpPost("{reference}/dimension-values")]
        // public async Task<IActionResult> AddDimensionValues([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string reference, [FromBody] IEnumerable<DimensionValuesDto> dimensionValuesDto)
        // {
        //     if (!_userValidationService.CheckAuthorizationToken(authorization))
        //     {
        //         return Unauthorized();
        //     }
        //     if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
        //     {
        //         return Unauthorized();
        //     }
        //     var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

        //     object[] array = new object[2];

        //     array[0] = reference;
        //     List<DimensionValuesDto> dtos = dimensionValuesDto.ToList();

        //     var sb = new StringBuilder();

        //     for (var j = 0; j < dtos.Count; j++)
        //     {
        //         sb.Append("{Possible Heights: ");
        //         sb.Append(EnumerableUtils.convert(dtos[j].PossibleHeights));
        //         sb.Append(". Possible Widths: ");
        //         sb.Append(EnumerableUtils.convert(dtos[j].PossibleWidths));
        //         sb.Append(". Possible Depths: ");
        //         sb.Append(EnumerableUtils.convert(dtos[j].PossibleDepths));
        //         sb.Append("}");
        //     }

        //     array[1] = sb.ToString();
        //     _logger.logInformation(userRef, LoggingEvents.PostItem, "Adding Dimension Values By Reference: {0} -- {1}", array);
        //     ValidationOutput validationOutput = _productService.AddVariousDimensionValues(reference, dimensionValuesDto);
        //     if (validationOutput.HasErrors())
        //     {
        //         if (validationOutput is ValidationOutputBadRequest)
        //         {
        //             _logger.logCritical(userRef, LoggingEvents.PostBadRequest, "Adding Dimension Values Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
        //             return BadRequest(validationOutput.FoundErrors);
        //         }

        //         if (validationOutput is ValidationOutputNotFound)
        //         {
        //             _logger.logCritical(userRef, LoggingEvents.PostNotFound, "Adding Dimension Values Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
        //             return NotFound(validationOutput.FoundErrors);
        //         }

        //         _logger.logCritical(userRef, LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
        //         return BadRequest("Type of validation output not recognized. Please contact your software provider.");
        //     }
        //     else
        //     {
        //         _logger.logInformation(userRef, LoggingEvents.PostOk, "Adding Dimension Values to Product Succeeded: {0}", sb.ToString());
        //         return Ok(validationOutput.DesiredReturn);
        //     }
        // }

        [HttpPost("{reference}/dimensions")]
        public async Task<IActionResult> AddDimensionValues([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string reference, [FromBody] DimensionValuesDto dimensionValuesDto)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            object[] array = new object[2];

            array[0] = reference;
            array[1] = dimensionValuesDto.Reference;

            _logger.logInformation(userRef, LoggingEvents.PostItem, "Adding Dimension Values By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _productService.AddDimensionValues(reference, dimensionValuesDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostBadRequest, "Adding Dimension Values Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostNotFound, "Adding Dimension Values Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.PostOk, "Adding Dimension Values to Product Succeeded: {0}", dimensionValuesDto.Reference);
                return Ok(validationOutput.DesiredReturn);
            }
        }

        [HttpPost("{productReference}/dimensions/{dimensionReference}/dimensions-values")]
        public async Task<IActionResult> AddValueElement([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string productReference, [FromRoute] string dimensionReference, [FromBody] DimensionValuesDto dimensionValuesDto)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            object[] array = new object[2];

            array[0] = productReference;
            array[1] = dimensionValuesDto.ToString();
            _logger.logInformation(userRef, LoggingEvents.PostItem, "Adding Dimension Values By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _productService.AddValueElement(productReference, dimensionReference, dimensionValuesDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostBadRequest, "Adding Dimension Values Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostNotFound, "Adding Dimension Values Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.PostOk, "Adding Dimension Values to Product Succeeded: {0}", array[1]);
                return Ok(validationOutput.DesiredReturn);
            }
        }

        [HttpPost("{productReference}/dimensions/{dimensionReference}/dimensions-algorithms")]
        public async Task<IActionResult> AddDimensionAlgorithm([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string productReference, [FromRoute] string dimensionReference, [FromBody] DimensionAlgorithmDto dimensionAlgorithmDto)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            object[] array = new object[3];

            array[0] = productReference;
            array[1] = dimensionReference;
            array[2] = dimensionAlgorithmDto.ToString();

            _logger.logInformation(userRef, LoggingEvents.PostItem, "Adding Dimension Algorithm By Reference: {0} -- {1} -- {2}", array);
            ValidationOutput validationOutput = _productService.AddDimensionAlgorithm(productReference, dimensionReference, dimensionAlgorithmDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostBadRequest, "Adding Dimension Algorithm Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostNotFound, "Adding Dimension Algorithm Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.PostOk, "Adding Dimension Algorithm to Product Succeeded: {0}", array[1]);
                return Ok(validationOutput.DesiredReturn);
            }
        }

        [HttpPost("{reference}/restriction")]
        public async Task<IActionResult> AddRestriction([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string reference, [FromBody] AlgorithmDto restriction)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            object[] array = new object[2];
            array[0] = reference;
            array[1] = restriction.ToString();
            _logger.logInformation(userRef, LoggingEvents.PostItem, "Adding Restriction By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _productService.AddRestriction(reference, restriction);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostBadRequest, "Adding Restriction Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostNotFound, "Adding Restriction Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.PostOk, "Adding Restriction to Product Succeeded: {0}", "");
                return Ok(validationOutput.DesiredReturn);
            }
        }

        [HttpPost("{reference}/part-restriction")]
        public async Task<IActionResult> AddRestriction([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string reference, [FromBody] PartDto partRestriction)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            object[] array = new object[2];
            array[0] = reference;
            array[1] = partRestriction.ToString();
            _logger.logInformation(userRef, LoggingEvents.PostItem, "Adding Restriction By Reference: {0} -- {1}", array);
            ValidationOutput validationOutput = _productService.AddPartRestriction(reference, partRestriction);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostBadRequest, "Adding Restriction Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.PostNotFound, "Adding Restriction Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.PostInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                var desiredReturn = (ProductDto)validationOutput.DesiredReturn;
                _logger.logInformation(userRef, LoggingEvents.PostOk, "Adding Restriction to Product Succeeded: {0}", desiredReturn.Reference);
                return Ok(desiredReturn);
            }
        }

        // ========= GET METHODS =========

        /**
         * GET method that will return all existent products in the system.
         */
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromHeader(Name = "Authorization")] string authorization)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.Validate(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }

            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            IEnumerable<ProductDto> list = _productService.GetAll();
            _logger.logInformation(userRef, LoggingEvents.GetAllOk, "Getting All Products: {0}", EnumerableUtils.convert(list));
            return Ok(list);
        }
        
        /**
         * GET method that will return all existent products in the system.
         */
        [HttpGet("top/structure")]
        public async Task<IActionResult> GetAllProductsStructure([FromHeader(Name = "Authorization")] string authorization)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.Validate(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }

            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            IEnumerable<ProductDto> list = _productService.GetAllStructure();
            _logger.logInformation(userRef, LoggingEvents.GetAllOk, "Getting All Products: {0}", EnumerableUtils.convert(list));
            return Ok(list);
        }

        /**
         * GET method that will return the product with the given reference.
         */
        [HttpGet("{reference}", Name = "GetProduct")]
        public async Task<IActionResult> GetProduct([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string reference)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }

            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            ValidationOutput validationOutput;

            if (await _userValidationService.ValidateContentManager(authorization.Split(" ")[1]))
            {
                _logger.logInformation(userRef, LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
                validationOutput = _productService.GetByReference(reference);
            }
            else if (await _userValidationService.Validate(authorization.Split(" ")[1]))
            {
                _logger.logInformation(userRef, LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
                validationOutput = _productService.ClientGetByReference(reference);
            }
            else
            {
                return Unauthorized();
            }

            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemBadRequest, "Getting Product Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemNotFound, "Getting Product Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.GetItemOk, "Getting Product: {0}", ((ProductDto)validationOutput.DesiredReturn).ToString());
                return Ok((ProductDto)validationOutput.DesiredReturn);
            }
        }

        /**
         * GET method that will return all the materials that were associated with the product with the passed reference.
         */
        [HttpGet("{reference}/materials")]
        public async Task<IActionResult> GetMaterialsAvailableToProduct([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string reference)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.Validate(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }

            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            _logger.logInformation(userRef, LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
            ValidationOutput validationOutput = _productService.GetMaterialsAvailableToProduct(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemBadRequest, "Getting Materials Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemNotFound, "Getting Materials Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.GetItemOk, "Getting Materials: {0}",
                    EnumerableUtils.convert((List<MaterialDto>)validationOutput.DesiredReturn));
                return Ok((List<MaterialDto>)validationOutput.DesiredReturn);
            }
        }

        /**
         * GET method that will return all the products that can complement the product with the passed reference.
         */
        [HttpGet("{reference}/parts")]
        public async Task<IActionResult> GetPartProductsAvailableToProduct([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string reference)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.Validate(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }

            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            _logger.logInformation(userRef, LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
            ValidationOutput validationOutput = _productService.GetPartProductsAvailableToProduct(reference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemBadRequest, "Getting Parts Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemNotFound, "Getting Parts Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.GetItemOk, "Getting Parts: {0}", EnumerableUtils.convert((List<ProductDto>)validationOutput.DesiredReturn));
                return Ok((List<ProductDto>)validationOutput.DesiredReturn);
            }
        }
        // ========= PUT METHODS =========

        /**
         * PUT method that will update the name, the description and the price of the product with the passed reference.
         */
        [HttpPut("{reference}")]
        public async Task<IActionResult> UpdateProduct([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string reference, [FromBody] ProductDto productDto)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            _logger.logInformation(userRef, LoggingEvents.UpdateItem, "Updating By Reference: {0}", reference);
            ValidationOutput validationOutput = _productService.Update(reference, productDto);
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
                if (validationOutput is ValidationOutputForbidden)
                {
                    _logger.logCritical(userRef, LoggingEvents.UpdateForbidden, "Updating Failed: {0}", ((ValidationOutputForbidden)validationOutput).ToString());
                    return new ForbiddenObjectResult(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.UpdateInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.UpdateNoContent, "Updating Product: {0}", ((ProductDto)validationOutput.DesiredReturn).ToString());
                _logger.logInformation(userRef, LoggingEvents.UpdateOk, "Updating Product: {0}", ((ProductDto)validationOutput.DesiredReturn).ToString());
                return NoContent();
            }
        }

        [HttpPut("{reference}/modelGroup")]
        public async Task<IActionResult> UpdateModelGroup(/* [FromHeader(Name = "Authorization")] string authorization, */ [FromRoute] string reference, [FromBody] ModelGroupDto dto)
        {
            // if (!_userValidationService.CheckAuthorizationToken(authorization))
            // {
            //     return Unauthorized();
            // }
            // if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
            // {
            //     return Unauthorized();
            // }
            // var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            ValidationOutput validationOutput = _productService.UpdateModelGroup(reference, dto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    return NotFound(validationOutput.FoundErrors);
                }

                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                return NoContent();
            }
        }

        // ========= DELETE METHODS =========

        /**
         * DELETE method that will soft-delete the product with the passed reference.
         */
        [HttpDelete("{reference}")]
        public async Task<IActionResult> DeleteProduct([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string reference)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            _logger.logInformation(userRef, LoggingEvents.SoftDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput = _productService.Remove(reference);
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
                _logger.logInformation(userRef, LoggingEvents.DeleteNoContent, "Deleting Product: {0}", reference);
                _logger.logInformation(userRef, LoggingEvents.SoftDeleteOk, "Deleting Product: {0}", reference);
                return NoContent();
            }
        }

        /*
         * DELETE method that will delete the list of passed materials from the product with the passed reference.
         */
        [HttpDelete("{reference}/materials")]
        public async Task<IActionResult> DeleteMaterials([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string reference, [FromBody]  IEnumerable<MaterialDto> enumerableMaterial)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            _logger.logInformation(userRef, LoggingEvents.HardDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput = _productService.DeleteMaterialsAndRespectiveAlgorithms(reference, enumerableMaterial);
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
                _logger.logInformation(userRef, LoggingEvents.DeleteNoContent, "Deleting Materials From Product: {0}", EnumerableUtils.convert(enumerableMaterial));
                _logger.logInformation(userRef, LoggingEvents.HardDeleteOk, "Deleting Materials From Product: {0}", EnumerableUtils.convert(enumerableMaterial));
                return NoContent();
            }
        }

        /*
         * DELETE method that will delete the list of passed parts from the product with the passed reference.
         */
        [HttpDelete("{reference}/parts")]
        public async Task<IActionResult> DeleteParts([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string reference, [FromBody] IEnumerable<ProductDto> enumerableProductReference)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            _logger.logInformation(userRef, LoggingEvents.HardDeleteItem, "Deleting By Reference: {0}", reference);
            ValidationOutput validationOutput = _productService.DeleteProductsAndRespectiveAlgorithms(reference, enumerableProductReference);
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
                _logger.logInformation(userRef, LoggingEvents.DeleteNoContent, "Deleting Parts From Product: {0}", EnumerableUtils.convert(enumerableProductReference));
                _logger.logInformation(userRef, LoggingEvents.HardDeleteOk, "Deleting Parts From Product: {0}", EnumerableUtils.convert(enumerableProductReference));
                return NoContent();
            }
        }

        /*
         * DELETE method that will delete the list of dimension values from the product with the passed reference.
         */
        // [HttpDelete("{reference}/dimension-values")]
        // public async Task<IActionResult> DeleteDimensionValues([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string reference, [FromBody] IEnumerable<DimensionValuesDto> enumerableDimensionValuesDto)
        // {
        //     if (!_userValidationService.CheckAuthorizationToken(authorization))
        //     {
        //         return Unauthorized();
        //     }
        //     if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
        //     {
        //         return Unauthorized();
        //     }
        //     var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

        //     _logger.logInformation(userRef, LoggingEvents.HardDeleteItem, "Deleting By Reference: {0}", reference);
        //     ValidationOutput validationOutput = _productService.DeleteVariousDimensionValues(reference, enumerableDimensionValuesDto);
        //     if (validationOutput.HasErrors())
        //     {
        //         if (validationOutput is ValidationOutputBadRequest)
        //         {
        //             _logger.logCritical(userRef, LoggingEvents.DeleteBadRequest, "Deleting Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
        //             return BadRequest(validationOutput.FoundErrors);
        //         }

        //         if (validationOutput is ValidationOutputNotFound)
        //         {
        //             _logger.logCritical(userRef, LoggingEvents.DeleteNotFound, "Deleting Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
        //             return NotFound(validationOutput.FoundErrors);
        //         }

        //         _logger.logCritical(userRef, LoggingEvents.DeleteInternalError, "Type of validation output not recognized. Please contact your software provider.");
        //         return BadRequest("Type of validation output not recognized. Please contact your software provider.");
        //     }
        //     else
        //     {
        //         _logger.logInformation(userRef, LoggingEvents.DeleteNoContent, "Deleting Dimension Values From Product: {0}", EnumerableUtils.convert(enumerableDimensionValuesDto));
        //         _logger.logInformation(userRef, LoggingEvents.HardDeleteOk, "Deleting Dimension Values From Product: {0}", EnumerableUtils.convert(enumerableDimensionValuesDto));
        //         return NoContent();
        //     }
        // }

        [HttpDelete("{productReference}/dimensions/{dimensionReference}")]
        public async Task<IActionResult> DeleteDimensionValues([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string productReference, [FromRoute] string dimensionReference)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            _logger.logInformation(userRef, LoggingEvents.HardDeleteItem, "Deleting DimensionValues By Reference: {0}", productReference);
            ValidationOutput validationOutput = _productService.DeleteDimensionValues(productReference, dimensionReference);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.DeleteBadRequest, "Deleting DimensionValues Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.DeleteNotFound, "Deleting DimensionValues Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.DeleteInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                _logger.logInformation(userRef, LoggingEvents.DeleteNoContent, "Deleting Dimension Values From Product: {0}", dimensionReference);
                _logger.logInformation(userRef, LoggingEvents.HardDeleteOk, "Deleting Dimension Values From Product: {0}", dimensionReference);
                return NoContent();
            }
        }

        [HttpDelete("{productReference}/dimensions/{dimensionReference}/dimensions-values")]
        public async Task<IActionResult> DeleteValueElement([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string productReference, [FromRoute] string dimensionReference, [FromBody] DimensionValuesDto dimensionValuesDto)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            _logger.logInformation(userRef, LoggingEvents.HardDeleteItem, "Deleting Dimension Values By Reference: {0}", productReference);
            ValidationOutput validationOutput = _productService.DeleteValueElement(productReference, dimensionReference, dimensionValuesDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.DeleteBadRequest, "Deleting Dimension Values Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.DeleteNotFound, "Deleting Dimension Values Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.DeleteInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                var desiredReturn = (DimensionValuesDto)validationOutput.DesiredReturn;
                _logger.logInformation(userRef, LoggingEvents.DeleteNoContent, "Deleting Dimension Values From Product: {0} ", desiredReturn);
                _logger.logInformation(userRef, LoggingEvents.HardDeleteOk, "Deleting Dimension Values From Product: {0}", desiredReturn);
                return NoContent();
            }
        }

        [HttpDelete("{productReference}/dimensions/{dimensionReference}/dimensions-algorithms")]
        public async Task<IActionResult> DeleteDimensionAlgorithm([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string productReference, [FromRoute] string dimensionReference, [FromBody] DimensionAlgorithmDto dimensionAlgorithmDto)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }
            if (!(await _userValidationService.ValidateContentManager(authorization.Split(" ")[1])))
            {
                return Unauthorized();
            }
            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);

            _logger.logInformation(userRef, LoggingEvents.HardDeleteItem, "Deleting DimensionValues Algorithm By Reference: {0}", productReference);
            ValidationOutput validationOutput = _productService.DeleteDimensionAlgorithm(productReference, dimensionReference, dimensionAlgorithmDto);
            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.DeleteBadRequest, "Deleting DimensionValues Algorithm Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.DeleteNotFound, "Deleting DimensionValues Algorithm Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.DeleteInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                var desiredReturn = (DimensionValuesDto)validationOutput.DesiredReturn;
                _logger.logInformation(userRef, LoggingEvents.DeleteNoContent, "Deleting Dimension Values From Product: {0} ", desiredReturn);
                _logger.logInformation(userRef, LoggingEvents.HardDeleteOk, "Deleting Dimension Values From Product: {0}", desiredReturn);
                return NoContent();
            }
        }

        [HttpGet("{reference}/products")]
        public async Task<IActionResult> GetProductsParts([FromHeader(Name = "Authorization")] string authorization, [FromRoute] string reference)
        {
            if (!_userValidationService.CheckAuthorizationToken(authorization))
            {
                return Unauthorized();
            }

            var userRef = await _userValidationService.GetUserRef(authorization.Split(" ")[1]);
            ValidationOutput validationOutput;

            _logger.logInformation(userRef, LoggingEvents.GetItem, "Getting By Reference: {0}", reference);
            validationOutput = _productService.GetProductsParts(reference);

            if (validationOutput.HasErrors())
            {
                if (validationOutput is ValidationOutputBadRequest)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemBadRequest, "Getting Parts Product Failed: {0}", ((ValidationOutputBadRequest)validationOutput).ToString());
                    return BadRequest(validationOutput.FoundErrors);
                }

                if (validationOutput is ValidationOutputNotFound)
                {
                    _logger.logCritical(userRef, LoggingEvents.GetItemNotFound, "Getting Parts Product Failed: {0}", ((ValidationOutputNotFound)validationOutput).ToString());
                    return NotFound(validationOutput.FoundErrors);
                }

                _logger.logCritical(userRef, LoggingEvents.GetItemInternalError, "Type of validation output not recognized. Please contact your software provider.");
                return BadRequest("Type of validation output not recognized. Please contact your software provider.");
            }
            else
            {
                var list = (List<ProductDto>)validationOutput.DesiredReturn;
                _logger.logInformation(userRef, LoggingEvents.GetItemOk, "Getting Products: {0}", EnumerableUtils.convert(list));
                return Ok(list);
            }
        }

    }
}