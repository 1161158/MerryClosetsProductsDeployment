using System.Collections.Generic;
using AutoMapper;
using MerryClosets.Models.DTO;
using MerryClosets.Models.Category;
using MerryClosets.Repositories.Interfaces;
using MerryClosets.Services.Interfaces;
using MerryClosets.Models.DTO.DTOValidators;
using MerryClosets.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using MerryClosets.Models.Product;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation.PredefinedTransformations;

namespace MerryClosets.Services.EF
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;

        private readonly ICategoryRepository _categoryRepository;

        private readonly CategoryDTOValidator _categoryDTOValidator;

        private readonly IProductService _productService;

        public CategoryService(IMapper mapper, ICategoryRepository categoryRepository,
            CategoryDTOValidator categoryDTOValidator, IProductService productService)

        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _categoryDTOValidator = categoryDTOValidator;
            _productService = productService;
        }

        /**
         * Private method used to verify the existence of category in the DB, through its unique reference.
         */
        private bool Exists(string reference)
        {
            var category = _categoryRepository.GetByReference(reference);
            return category != null;
        }
        
        private bool ExistsAndIsActive(string reference)
        {
            var category = _categoryRepository.GetByReference(reference);
            if (category != null && category.IsActive)
            {
                return true;
            }

            return false;
        }

        // ============ Methods to CREATE something ============

        /**
         * Method that will validate and create a new category in the database.
         *
         * Validations performed:
         * 1. Validation of the new category's reference (business rules);
         * 2. Validation of the new category's reference (database);
         * 3. Validation of the received info. (name, description, parent category) (business rules)
         * 4. Validation of the category's parent category reference (database)
         */
        public ValidationOutput Register(CategoryDto dto)
        {
            //1.
            ValidationOutput validationOutput = _categoryDTOValidator.DTOReferenceIsValid(dto.Reference);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            //2.
            validationOutput = new ValidationOutputBadRequest();
            if (Exists(dto.Reference))
            {
                validationOutput.AddError("Category's reference",
                    "A category with the reference '" + dto.Reference + "' already exists in the system!");
                return validationOutput;
            }

            //3.
            validationOutput = _categoryDTOValidator.DTOIsValidForRegister(dto);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            //4.
            validationOutput = new ValidationOutputNotFound();
            if (!string.IsNullOrEmpty(dto.ParentCategoryReference))
            {
                if (!Exists(dto.ParentCategoryReference))
                {
                    validationOutput.AddError("Category's parent category reference",
                        "No category with the reference '" + dto.ParentCategoryReference + "' exists in the system.");
                    return validationOutput;
                }
            }

            //If we reached here than that means we can add the new category
            Category passedCategory = _mapper.Map<Category>(dto);
            validationOutput.DesiredReturn = _mapper.Map<CategoryDto>(_categoryRepository.Add(passedCategory));

            return validationOutput;
        }

        // ============ Methods to GET something ============

        /**
         * Method that will return either the category in the form of a DTO that has the passed reference OR all the errors found when trying to do so.
         * 
         * Validations performed:
         * 1. Validation of the passed category's reference (database);
         * 
         * This method can return a soft-deleted category.
         */
        public ValidationOutput GetByReference(string refer)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!Exists(refer))
            {
                validationOutput.AddError("Category's reference",
                    "No category with the reference '" + refer + "' exists in the system.");
                return validationOutput;
            }

            validationOutput.DesiredReturn = _mapper.Map<CategoryDto>(_categoryRepository.GetByReference(refer));
            return validationOutput;
        }
        
        /**
        * Method that will return either the category in the form of a DTO that has the passed reference OR all the errors found when trying to do so.
        * 
        * Validations performed:
        * 1. Validation of the passed category's reference (database);
        * 
        * This method can return a soft-deleted category.
        */
        public ValidationOutput ClientGetByReference(string refer)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!ExistsAndIsActive(refer))
            {
                validationOutput.AddError("Category's reference",
                    "No category with the reference '" + refer + "' exists in the system.");
                return validationOutput;
            }

            validationOutput.DesiredReturn = _mapper.Map<CategoryDto>(_categoryRepository.GetByReference(refer));
            return validationOutput;
        }

        /**
         * Method that will return all categories present in the system, each in the form of a DTO OR all the errors found when trying to do so.
         *
         * May return an empty list, indicating that there are no categories in the system (yet).
         * This list will not include soft-deleted categories.
         */
        public IEnumerable<CategoryDto> GetAll()
        {
            List<CategoryDto> categoryDtoList = new List<CategoryDto>();
            List<Category> categoryList = _categoryRepository.List();

            //For-each just to convert each Category object into a CategoryDto object
            foreach (var category in categoryList)
            {
                categoryDtoList.Add(_mapper.Map<CategoryDto>(category));
            }

            return categoryDtoList;
        }

        // ============ Methods to UPDATE something ============

        /**
         * Method that will update the category (name and description), with the passed reference, with the data present in the passed DTO OR return all the errors found when trying to do so.
         *
         * Validations performed:
         * 1. Validation of the passed category's reference (database);
         * 2. Validation of the name and description of the passed DTO.
         */
        public ValidationOutput Update(string refer, CategoryDto dto)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!Exists(refer))
            {
                validationOutput.AddError("Reference of category to update",
                    "No category with the reference '" + refer + "' exists in the system.");
                return validationOutput;
            }

            validationOutput = new ValidationOutputForbidden();
            if (dto.Reference != null && !dto.Reference.Equals(refer))
            {
                validationOutput.AddError("Reference of category", "It's not allowed to update reference.");
                return validationOutput;
            }
            
            //2.
            validationOutput = _categoryDTOValidator.DTOIsValidForUpdate(dto);
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            Category categoryToUpdate = _categoryRepository.GetByReference(refer);

            if (dto.Name != null)
            {
                categoryToUpdate.Name = dto.Name;
            }

            if (dto.Description != null)
            {
                categoryToUpdate.Description = dto.Description;
            }

            validationOutput.DesiredReturn = _mapper.Map<CategoryDto>(_categoryRepository.Update(categoryToUpdate));
            return validationOutput;
        }

        // ============ Methods to REMOVE something ============

        /**
         * Method that will soft-delete the category with the passed reference OR return all the errors found when trying to do so.
         * 
         * Validations performed:
         * 1. Validation of the passed category's reference (database);
         */
        public ValidationOutput Remove(string refer)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!Exists(refer))
            {
                validationOutput.AddError("Reference of category to remove",
                    "No category with the reference '" + refer + "' exists in the system.");
                return validationOutput;
            }

            Category categoryToRemove = _categoryRepository.GetByReference(refer);

            ///////////////
            // Lista de todas as categorias a apagar
            var listCategories = new List<Category>();
            // Lista de todos os produtos a apagar
            var listProducts = new List<Product>();


            // Lista de categorias filhas da categoria a apagar
            var validationOutputChildCategories = ObtainDirectChildCategories(refer);
            /*if (!validationOutputChildCategories.HasErrors())
            {
                var childCategories = (List<CategoryDto>) validationOutputChildCategories.DesiredReturn;
                foreach (var categoryChild in childCategories)
                {
                    Console.WriteLine("Category-> " + categoryToRemove.Reference + "----- categoryChild -> " + categoryChild.Reference);
                    listCategories.Add(_mapper.Map<Category>(categoryChild));
                    //GetChildCategories(_mapper.Map<Category>(categoryChild), listCategories, listProducts);
                }
            }*/

            //Preenche as listas de produtos e categorias
            //GetChildCategories(categoryToRemove, listCategories, listProducts);

            /*foreach (var VARIABLE in listProducts)
            {
                Console.WriteLine(VARIABLE.Reference);
            }

            listCategories.Reverse();
            foreach (var VARIABLE in listCategories)
            {
                Console.WriteLine(VARIABLE.Reference);
            }*/

            //_categoryRepository.Delete(listCategories[0]);
/*            
            
            // Adiciona à lista de produtos a remover os produtos da categoria que se pretende apagar
            foreach (var product in GetProductsOfCategory(_mapper.Map<Category>(categoryToRemove)))
            {
                listProducts.Add(_mapper.Map<Product>(product));
            }
            
            /*Console.WriteLine("N Produtos: " + listProducts.Count);
            foreach (var product in listProducts)
            {
                Console.WriteLine("A Remover Produto " + product.Reference);
                _productService.Remove(product.Reference);
                Console.WriteLine("Já Removeu Produto " + product.Reference);
            }*/

            /*_categoryRepository.Delete(categoryToRemove);
            Console.WriteLine("Já Removeu Categoria " + categoryToRemove.Reference);*/
/*
            Console.WriteLine("N Categorias: " + listCategories.Count);
            listCategories.Reverse();
            foreach (var category in listCategories)
            {
                Console.WriteLine(_categoryRepository.Update(category).Reference);
                /*
                Console.WriteLine("A remover categoria " + category.Reference + " da lista");
                listCategories.Remove(category);
                Console.WriteLine("Já removeu categoria " + category.Reference + " da lista");
                Console.WriteLine(category.Version);
                Console.WriteLine("A remover categoria " + category.Reference);
                _categoryRepository.Delete(category);
                Console.WriteLine("Já removeu categoria " + category.Reference);
            }*/
            ///////////////

            /*Console.WriteLine("||||||||||||||||||||||||||||");


            _productService.Remove("pro3");
            _productService.Remove("pro3");
            _productService.Remove("pro4");

            _categoryRepository.Delete(_categoryRepository.GetByReference("cat4"));
            _categoryRepository.Delete(_categoryRepository.GetByReference("cat3"));
            _categoryRepository.Delete(_categoryRepository.GetByReference("cat2"));
            
            Console.WriteLine("||||||||||||||||||||||||||||");*/

            _categoryRepository.Delete(categoryToRemove);
            //_categoryRepository.Delete(_categoryRepository.GetByReference("cat4"));*/
            return validationOutput;
        }

        // ============ Business Methods ============

        /**
         * This method will either return all the categories that have the category with the passed reference as their parent OR return all the errors found when trying to do so.
         * May return an empty list, indicating that there are no child categories (yet).
         * 
         * Validations performed:
         * 1. Validation of the passed category's reference (database);
         */
        public ValidationOutput ObtainDirectChildCategories(string refer)
        {
            //1.
            ValidationOutput validationOutput = new ValidationOutputNotFound();
            if (!Exists(refer))
            {
                validationOutput.AddError("Category reference",
                    "No category with the reference '" + refer + "' exists in the system.");
                return validationOutput;
            }

            List<CategoryDto> childCategoriesDtoList = new List<CategoryDto>();
            List<Category> childCategoriesList = _categoryRepository.DirectChildCategories(refer);

            //For-each just to convert each Category object into a CategoryDto object
            foreach (var category in childCategoriesList)
            {
                childCategoriesDtoList.Add(_mapper.Map<CategoryDto>(category));
            }

            validationOutput.DesiredReturn = childCategoriesDtoList;
            return validationOutput;
        }

        /**
         * Método recursivo 
         */
        private void GetChildCategories(Category categoryChild, List<Category> listCategories,
            List<Product> listProducts)
        {
            listCategories.Add(categoryChild);

            //Console.WriteLine("\nPRODUTOS\n");
            foreach (var product in GetProductsOfCategory(categoryChild))
            {
                //Console.WriteLine("Category-> " + categoryChild.Reference + "----- product -> " + product.Reference);
                listProducts.Add(_mapper.Map<Product>(product));
                //Console.WriteLine("A remover produto");
                //_productService.Remove(product.Reference);
                //Console.WriteLine("Já removeu produto");
            }

            
            var validationOutputChildCategories = ObtainDirectChildCategories(categoryChild.Reference);
            
            if (!validationOutputChildCategories.HasErrors())
            {
                var childCategories = (new List<CategoryDto>());
                foreach (var categoryDto in childCategories)
                {
                    var category = _mapper.Map<Category>(categoryDto);
                    Console.WriteLine("Category-> " + categoryChild.Reference + "----- categoryChild -> " +
                                      categoryDto);
                    GetChildCategories(category, listCategories, listProducts);

                    /*Console.WriteLine("\nCATEGORIA\n");
                    Console.WriteLine("A remover categoria " + category.Reference);
                    _categoryRepository.Delete(category);
                    Console.WriteLine("Já removeu categoria " + category.Reference);*/
                }
            }
        }

        private List<ProductDto> GetProductsOfCategory(Category category)
        {
            var validationOutputProducts = _productService.GetProductsOfCategory(category.Reference);

            if (validationOutputProducts.HasErrors()) return new List<ProductDto>();

            var list = (List<ProductDto>) validationOutputProducts.DesiredReturn;
            return new List<ProductDto>(list);
        }
    }
}