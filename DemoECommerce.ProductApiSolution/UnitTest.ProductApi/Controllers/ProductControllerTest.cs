using eCommerce.SharedLibarary.Responses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Persentation.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.ProductApi.Controllers
{
    public class ProductControllerTest
    {
        private readonly IProduct _productInterface;

        private readonly ProductsController _productsController;


        public ProductControllerTest()
        {
            _productInterface=A.Fake<IProduct>();

            _productsController = new ProductsController(_productInterface);
        }

        [Fact]
        public async Task GetsProduct_WhenProductExists_ReturnOkResponseWithProducts()
        {
            //arrange
            var products = new List<Product>()
            {
                new(){Id=1, Name="Product1" ,Quantity=10,Price=300.4m},
                new(){Id=2, Name="Product2" ,Quantity=110,Price=1300.4m}

            };

            // set up fake response
            A.CallTo(()=>_productInterface.GetAllAsync()).Returns(products);

            //act
            var result = await _productsController.GetProducts();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedProducts = okResult.Value as IEnumerable<ProductDto>;
            returnedProducts.Should().NotBeNull();
            returnedProducts.Count().Should().Be(2);
            returnedProducts.First().Id.Should().Be(1);
            returnedProducts.Last().Id.Should().Be(2);
;
        }
        [Fact]
        public async Task GetsProduct_WhenNoProductExists_ReturnNotFoundResponse()
        {
            //arrange
            var products = new List<Product>();


            //setup fake resp
            A.CallTo(() => _productInterface.GetAllAsync()).Returns(products);

            //act
            var result=await _productsController.GetProducts();
            var notFoundResult= result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            var message = notFoundResult.Value as string;
            message.Should().Be("No products detected");
        }

        [Fact]

        public async Task CreateProduct_WhenModelStateIsInvalid_ReturnBadRequest()
        {

            //arrange
            var productDto = new ProductDto(1, "Product 1", 34, 66.9m);
            _productsController.ModelState.AddModelError("Name", "Required");

            //act

            var result = await _productsController.CreateProduct(productDto);

            //assert

            var badRequestResult= result.Result as BadRequestObjectResult;

            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);



        }


        [Fact]
        public async Task CreateProduct_WhenModelStateIsValid_ReturnOK()
        {

            //arrange
            var productDto = new ProductDto(1, "Product 1", 34, 66.9m);
            var response = new Response(true, "Created");

            //act
            A.CallTo(()=>_productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
            var result = await _productsController.CreateProduct(productDto);

            //assert

            var okRequestResult = result.Result as OkObjectResult;

            okRequestResult.Should().NotBeNull();
            okRequestResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okRequestResult.Value as Response;
            responseResult.Message.Should().Be("Created");
            responseResult.Flag.Should().BeTrue();



        }


        [Fact]
        public async Task CreateProduct_WhenCreateFails_ReturnBadRequest()
        {

            //arrange
            var productDto = new ProductDto(1, "Product 1", 34, 66.9m);
            var response = new Response(false, "Cant Be Created");

            //act
            A.CallTo(() => _productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
            var result = await _productsController.CreateProduct(productDto);

            //assert

            var failRequestResult = result.Result as BadRequestObjectResult;

            failRequestResult.Should().NotBeNull();
            failRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = failRequestResult.Value as Response;
            responseResult.Message.Should().Be("Cant Be Created");
            responseResult.Flag.Should().BeFalse();



        }


        [Fact]
        public async Task UpdateProduct_WhenModelStateIsInvalid_ReturnBadRequest()
        {
            //arrange
            var productDto = new ProductDto(1, "Product 1", 34, 66.9m);
            _productsController.ModelState.AddModelError("Name", "Required");


            //act

            var result = await _productsController.UpdateProduct(productDto);

            var badRequestResult= result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);


        }


        [Fact]
        public async Task UpdateProduct_WhenModelStateIsValid_ReturnOK()
        {
            //arrange
            var productDto = new ProductDto(1, "Product 1", 34, 66.9m);
            var response = new Response(true, "updated");

            //act
            A.CallTo(()=>_productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);

            var result = await _productsController.UpdateProduct(productDto);

            var okRequestResult = result.Result as OkObjectResult;
            okRequestResult.Should().NotBeNull();
            okRequestResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var messageResult = okRequestResult.Value as Response;
            messageResult.Message.Should().Be("updated");
            messageResult.Flag.Should().BeTrue();

        }

        [Fact]
        public async Task UpdateProduct_WhenCreateFails_ReturnBadRequest()
        {

            //arrange
            var productDto = new ProductDto(1, "Product 1", 34, 66.9m);
            var response = new Response(false, "Cant Be updated");

            //act
            A.CallTo(() => _productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);
            var result = await _productsController.UpdateProduct(productDto);

            //assert

            var failRequestResult = result.Result as BadRequestObjectResult;

            failRequestResult.Should().NotBeNull();
            failRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = failRequestResult.Value as Response;
            responseResult.Message.Should().Be("Cant Be updated");
            responseResult.Flag.Should().BeFalse();



        }

        [Fact]
        public async Task DeleteProduct_WhenModelStateIsInvalid_ReturnBadRequest()
        {
            //arrange
            var productDto = new ProductDto(1, "Product 1", 34, 66.9m);
            _productsController.ModelState.AddModelError("Name", "Required");


            //act

            var result = await _productsController.DeleteProduct(productDto);

            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }


        [Fact]
        public async Task DeleteProduct_WhenModelStateIsValid_ReturnOkRequest()
        {
            //arrange
            var productDto = new ProductDto(1, "Product 1", 34, 66.9m);
            var response = new Response(true, "deleted");

            //act
            A.CallTo(() => _productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);

            var result = await _productsController.DeleteProduct(productDto);

            var okRequestResult = result.Result as OkObjectResult;
            okRequestResult.Should().NotBeNull();
            okRequestResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var messageResult = okRequestResult.Value as Response;
            messageResult.Message.Should().Be("deleted");
            messageResult.Flag.Should().BeTrue();

        }


        [Fact]
        public async Task DeleteProduct_WhenDeleteFails_ReturnBadRequest()
        {

            //arrange
            var productDto = new ProductDto(1, "Product 1", 34, 66.9m);
            var response = new Response(false, "Cant Be deleted");

            //act
            A.CallTo(() => _productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);
            var result = await _productsController.DeleteProduct(productDto);

            //assert

            var failRequestResult = result.Result as BadRequestObjectResult;

            failRequestResult.Should().NotBeNull();
            failRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = failRequestResult.Value as Response;
            responseResult.Message.Should().Be("Cant Be deleted");
            responseResult.Flag.Should().BeFalse();



        }

    }
}
