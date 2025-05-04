using eCommerce.SharedLibarary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Persentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductsController : ControllerBase
    {
        private readonly IProduct _product;

        public ProductsController(IProduct product)
        {
            _product = product;
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products=await _product.GetAllAsync();

            if (products.Any())
            {
              var (_,productsDto)=  ProductConversion.FromEntity(null,products);

                return productsDto!.Any() ? Ok(productsDto) : NotFound("No Product found");
            }
            return NotFound("No products detected");
           
        }

        [HttpGet("{id:int}")]
        [Produces("application/json")]

        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product=await _product.FindByIdAsync(id);
            if(product is not null)
            {
                var (productDto,_) = ProductConversion.FromEntity(product, null);
                return productDto is not null ? Ok(productDto) : NotFound("No Product found");

            }
            return NotFound("No product with this id founded");
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<Response>> CreateProduct(ProductDto product)
        {

            if(ModelState.IsValid)
            {

                var getEntity=ProductConversion.ToEntity(product);
                var response=await _product.CreateAsync(getEntity);

                return response.Flag is true?Ok(response):BadRequest(response);
            }

            return BadRequest(ModelState);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<Response>> UpdateProduct(ProductDto product)
        {

            if(ModelState.IsValid)
            {

                var getEntity = ProductConversion.ToEntity(product);

             var response=  await  _product.UpdateAsync(getEntity);

                return response.Flag is true ? Ok(response) : BadRequest(response);


            }
            return BadRequest(ModelState);

        }


        [HttpDelete]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<Response>> DeleteProduct (ProductDto product)
        {
            if (ModelState.IsValid)
            {

                var getEntity= ProductConversion.ToEntity(product); 

                var response= await _product.DeleteAsync(getEntity);
                return response.Flag is true ? Ok(response) : BadRequest(response);

            }

            return BadRequest(ModelState);

        }

    }
}
