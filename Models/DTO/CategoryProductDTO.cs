using System;
using System.Collections.Generic;
using MerryClosets.Models.Collection;
using MerryClosets.Models.DTO;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class CategoryProductDto
        
    {
        public string CategoryName { get; set; }

        public List<ProductDto> Products { get; set; } = new List<ProductDto>();

        public CategoryProductDto(string categoryName, List<ProductDto> products)
        {
            this.CategoryName = categoryName;
            this.Products = products;
        }

        public CategoryProductDto(string categoryName)
        {
            this.CategoryName = categoryName;
        }

        protected CategoryProductDto(){}

        // override object.Equals
        public override bool Equals(object obj)
        {
            
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (CategoryProductDto) obj;
            
            return string.Equals(this.CategoryName, other.CategoryName, StringComparison.Ordinal);
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            return System.Tuple.Create(this.CategoryName).GetHashCode();;
        }
    }
}