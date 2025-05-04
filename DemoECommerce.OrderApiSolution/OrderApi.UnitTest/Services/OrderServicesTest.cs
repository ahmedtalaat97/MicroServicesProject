using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interface;
using OrderApi.Application.Services;
using OrderApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OrderApi.UnitTest.Services
{
    public class OrderServicesTest
    {
        private readonly IOrderService _orderServiceInterface;
        private readonly IOrder _orderInterface;
        private readonly OrderService _orderService;
        private readonly HttpClient _httpClient;

        public OrderServicesTest()
        {
            var productDto = new ProductDto(1, "prod1", 10, 100);
            _httpClient = CreateFakeHttpClient(productDto);
            _orderInterface = A.Fake<IOrder>();
            _orderServiceInterface = A.Fake<IOrderService>();
            _orderService = new OrderService(_httpClient, null, _orderInterface, null);
         
        }

        //Fake Http message handler
        public class FakeHttpMessageHandler(HttpResponseMessage response):HttpMessageHandler
        {
            private readonly HttpResponseMessage _response=response;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
         => Task.FromResult(_response) ;
        }

        //fake http client

        private static HttpClient CreateFakeHttpClient(object o)
        {
            var httpResponseMessage=new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content=JsonContent.Create(o)
            };
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);
            var _httpClient =new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress=new Uri("http://localhost")
            } ;
            return _httpClient;
        }


        //get product
        [Fact]

        public async Task GetProduct_ValidProductId_ReturnProduct()
        {
            //arrange
            int productId=1;



            //act
            var result = await _orderService.GetProduct(productId);



            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
        }

        [Fact]
        public async Task GetProduct_InValidProductId_ReturnNull()
        {
            int invalidProductId = -1;

            var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = JsonContent.Create<string>(null)
            };

            var httpClient = new HttpClient(new FakeHttpMessageHandler(httpResponse))
            {
                BaseAddress = new Uri("http://localhost")
            };

            var orderService = new OrderService(httpClient, null, null, null);

            // Act
            var result = await orderService.GetProduct(invalidProductId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetOrdersByClientId_WhenOrdersExist_ReturnOrderDetails()
        {
            // Arrange
            int clientId = 1;
                            var orders = new List<Order>
                    {
                        new() { Id = 1, ClientId = clientId, ProductId = 1, PurchaseQuantity = 1, OrderDate = DateTime.Now },
                        new() { Id = 2, ClientId = clientId, ProductId = 2, PurchaseQuantity = 1, OrderDate = DateTime.Now }
                    };

            var orderDtos = new List<OrderDto>()
            {new OrderDto(1, clientId, 1, 1, DateTime.Now), 
            new OrderDto(2, clientId, 2, 2, DateTime.Now)
            };
            
                

                            // Mock the repository
                            A.CallTo(() => _orderInterface.GetOrdersAsync(
                                A<Expression<Func<Order, bool>>>.Ignored)).Returns(orders);

                            // Mock the mapper
                            var mapper = A.Fake<IMapper>();
                            A.CallTo(() => mapper.Map<IEnumerable<OrderDto>>(orders))
                .Returns(orderDtos);

            var service = new OrderService(_httpClient, null, _orderInterface, mapper);

            var result = await service.GetOrdersByClientId(clientId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(orderDtos);
            result.Should().HaveCount(2);
        }
    }
}
