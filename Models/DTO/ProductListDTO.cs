using System.Collections.Generic;

namespace MerryClosets.Models.DTO
{
    public class ProductListDto
    {
        public List<ProductDto> ProductList { get; set; }

        public ProductListDto()
        {
        }

        public ProductListDto(List<ProductDto> productList)
        {
            ProductList = productList;
        }
    }
}