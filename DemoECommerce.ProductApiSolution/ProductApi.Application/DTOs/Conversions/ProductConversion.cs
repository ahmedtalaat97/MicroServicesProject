using ProductApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Application.DTOs.Conversions
{
   public  static class ProductConversion
    {
        public static Product ToEntity(ProductDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Quantity = dto.Quantity,
            Price = dto.Price,
        };
            

        public static (ProductDto? ,IEnumerable<ProductDto>?)FromEntity(Product? product,IEnumerable<Product?> products)
        {
            if(product is not null||products is null)
            {
                var singleProduct = new ProductDto(product.Id, product.Name, product.Quantity, product.Price);
                return (singleProduct,null);
               

            }

            if(products is not null || product is null)
            {
                var _products = products.Select(p => new ProductDto(p.Id, p.Name!,p.Quantity,p.Price)).ToList();

                return (null, _products);
            }
            return (null, null);
        }
    }
}
