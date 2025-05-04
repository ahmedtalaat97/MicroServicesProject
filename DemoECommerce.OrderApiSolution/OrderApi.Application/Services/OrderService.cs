using AutoMapper;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interface;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Services
{
    public class OrderService (HttpClient httpClient ,ResiliencePipelineProvider<string> resiliencePipeline,IOrder _order , IMapper _mapper) : IOrderService
    {

        // Ngeb el product el awl
        //Get Product

        public async Task<ProductDto> GetProduct(int productId)
        {
            // ha call el product Api
            //redirect el call dy lel Api getway 

            var getProduct = await httpClient.GetAsync($"/api/products/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null;
            var product = await getProduct.Content.ReadFromJsonAsync<ProductDto>();
            return product!;

        }

        //GET USER

        public async Task<AppUserDto> GetUser(int userId)
        {
            var getUser = await httpClient.GetAsync($"/api/authentication/{userId}");
            if (!getUser.IsSuccessStatusCode)
                return null;
            var user = await getUser.Content.ReadFromJsonAsync<AppUserDto>();
            return user!;
        }



        public async Task<OrderDetailsDto> GetOrderDetails(int orderId)
        {
            //prepare el order

            var order= await _order.FindByIdAsync(orderId);
            if (order is null || order!.Id <= 0)
                return null;

            // Get retry pipeline

            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");
            
            //prepare Product

            var productDto= await retryPipeline.ExecuteAsync(async token=>await GetProduct(order.ProductId));

            // Prepare Client

            var appUserDto=await retryPipeline.ExecuteAsync(async token=>await GetUser(order.ClientId));

            if (appUserDto == null)
            {
                // Optionally return partial details or just handle gracefully
                return new OrderDetailsDto(orderId, productDto.Id, 0, "Unknown", "N/A", "N/A", "N/A",
                    productDto.Name, order.PurchaseQuantity, productDto.Price,
                    productDto.Price * productDto.Quantity, order.OrderDate);
            }
            // popilate order details
            return new OrderDetailsDto(orderId, productDto.Id, appUserDto.Id, appUserDto.Name, appUserDto.Email, appUserDto.Address,appUserDto.TelephoneNumber,

                productDto.Name, order.PurchaseQuantity, productDto.Price, (productDto.Price*productDto.Quantity),order.OrderDate);

        }


        

        public async Task<IEnumerable<OrderDto>> GetOrdersByClientId(int clientId)
        {
            // Get all Clients orders

            var orders = await _order.GetOrdersAsync(o => o.ClientId == clientId);
            if (!orders.Any())
                return null;

            // convert el order l order dto

            var orderDto= _mapper.Map<IEnumerable<OrderDto>>(orders);
            return orderDto;

        }
    }
}
