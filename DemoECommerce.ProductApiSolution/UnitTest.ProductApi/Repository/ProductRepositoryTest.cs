using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.ProductApi.Repository
{
    public class ProductRepositoryTest
    {
        private readonly ProductDbContext _productDbContext;
        private readonly ProductRepository _productRepository;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>().UseInMemoryDatabase(databaseName: "ProductDb").Options;

            _productDbContext = new ProductDbContext(options);
            _productRepository = new ProductRepository(_productDbContext);
        }

        // Create Product
        [Fact]
        public async Task CreateAsync_WhenProductAlreadyExists_ReturnErrorResponse()
        {
            //arrange
            var existingProduct = new Product { Name = "ExistingProduct" };
            _productDbContext.Products.Add(existingProduct);
            await _productDbContext.SaveChangesAsync();

            //act

            var result = await _productRepository.CreateAsync(existingProduct);
            //assert 

            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("ExistingProduct already exist");



        }


        [Fact]
        public async Task CreateAsync_WhenProductDoesNotExists_ReturnTrueResponseAndAddProduct()
        {
            //arrange
            var newProduct = new Product { Name = "New Product" };

            //act

            var result = await _productRepository.CreateAsync(newProduct);
            //assert 

            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("New Product added to database successfully");



        }


        [Fact]
        public async Task DeleteAsync_ProductIsFound_ReturnSuccessResponse()
        {

            var product = new Product { Id = 1, Name = "Exist", Price = 100, Quantity = 10 };

            _productDbContext.Products.Add(product);


            var result = await _productRepository.DeleteAsync(product);
            result.Should().NotBeNull();

            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Exist is deleted successfully");


        }

        [Fact]
        public async Task DeleteAsync_ProductIsNotFound_ReturnFailedResponse()
        {

            var product = new Product { Id = 1, Name = "Exist", Price = 100, Quantity = 10 };


            _productDbContext.Products.Add(product);
            var product2 = new Product { Id = 2, Name = "notExist", Price = 100, Quantity = 10 };

            var result = await _productRepository.DeleteAsync(product2);
            result.Should().NotBeNull();

            result.Flag.Should().BeFalse();
            result.Message.Should().Be("notExist is not found");


        }

        [Fact]
        public async Task FindByIdAsync_WhenProductFound_ReturnProduct()
        {
            var product = new Product { Id = 10, Name = "Exist", Price = 100, Quantity = 10 };
            _productDbContext.Products.Add(product);
            await _productDbContext.SaveChangesAsync();

            //act
            var result = await _productRepository.FindByIdAsync(product.Id);

            //assert
            result.Should().NotBeNull();
            result.Id.Should().Be(product.Id);
            result.Name.Should().Be(product.Name);


        }


        [Fact]
        public async Task FindByIdAsync_WhenProductNotFound_ReturnProduct()
        {
            var product = new Product { Id = 2, Name = "NotExist", Price = 100, Quantity = 10 };

            //act
            var result = await _productRepository.FindByIdAsync(product.Id);

            //assert
            result.Should().BeNull();



        }

        [Fact]

        public async Task GetAllAsync_WhenProductsFound_ReturnProducts()
        {
            //arrange

            var product = new Product { Id = 20, Name = "New", Price = 100, Quantity = 10 };
            _productDbContext.Add(product);
            await _productDbContext.SaveChangesAsync();



            //act

            var result = await _productRepository.GetAllAsync();
            result.Should().NotBeNull().And.ContainSingle(p => p.Id == product.Id).Which.Should().BeEquivalentTo(product);

            result.Last().Should().BeEquivalentTo(product);


        }

        [Fact]

        public async Task GetAllAsync_WhenProductsNotFound_ReturnNull()
        {
            //arrange

            _productDbContext.Products.RemoveRange(await _productDbContext.Products.ToListAsync());
           await _productDbContext.SaveChangesAsync();


            //act

            var result = await _productRepository.GetAllAsync();

            result.Count().Should().Be(0);

        }


        [Fact]
        public async Task GetByAsync_WhenProductIsFound_ReturnProduct()
        {
            //arrange
            var product = new Product() { Id = 50, Name = "Iphone", Price = 100, Quantity = 10 };

            _productDbContext.Add(product);
            await _productDbContext.SaveChangesAsync();

            Expression<Func<Product, bool>> predicate = p => p.Name == "Iphone";

            //act
            var result = await _productRepository.GetByAsync(predicate);
            result.Should().NotBeNull();
            result.Id.Should().Be(50);


        }
        [Fact]
        public async Task GetByAsync_WhenProductIsNotFound_ReturnNull()
        {
            //arrange
        

            Expression<Func<Product, bool>> predicate = p => p.Name == "ggg";

            //act
            var result = await _productRepository.GetByAsync(predicate);
            result.Should().BeNull();


        }
    }
}
