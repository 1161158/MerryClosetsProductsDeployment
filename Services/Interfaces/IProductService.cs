using System.Collections.Generic;
using MerryClosets.Models.DTO;
using MerryClosets.Models.Product;
using Newtonsoft.Json.Linq;

namespace MerryClosets.Services.Interfaces
{
    public interface IProductService : IService<Product, ProductDto>
    {

        ValidationOutput GetPartProductsAvailableToProduct(string refer);
        ValidationOutput GetProductsOfCategory(string productReference);
        ValidationOutput GetMaterialsAvailableToProduct(string refer);

        ValidationOutput AddMaterialsAndRespectiveAlgorithms(string reference, IEnumerable<ProductMaterialDto> enumerableProductMaterialDto);
        ValidationOutput DeleteMaterialsAndRespectiveAlgorithms(string refer, IEnumerable<MaterialDto> enumerableMaterial);

        ValidationOutput AddProductsAndRespectiveAlgorithms(string refer, IEnumerable<PartDto> enumerablePartDto);
        ValidationOutput DeleteProductsAndRespectiveAlgorithms(string refer, IEnumerable<ProductDto> enumerableProduct);

        ValidationOutput AddVariousDimensionValues(string refer, IEnumerable<DimensionValuesDto> enumerableDimensionValuesDto);
        ValidationOutput DeleteVariousDimensionValues(string refer, IEnumerable<DimensionValuesDto> enumerableDimensionValuesDto);
        ValidationOutput AddRestriction(string refer, AlgorithmDto dto);
        ValidationOutput AddPartRestriction(string refer, PartDto dto);
    }
}