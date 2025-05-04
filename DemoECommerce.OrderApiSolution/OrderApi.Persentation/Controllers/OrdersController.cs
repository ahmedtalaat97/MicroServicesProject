using AutoMapper;
using eCommerce.SharedLibarary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interface;
using OrderApi.Application.Services;
using OrderApi.Domain.Entities;

namespace OrderApi.Persentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController(IOrder orderInterface , IOrderService orderService , IMapper _mapper ) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        { 
            var order= await orderInterface.GetAllAsync();
            if(!order.Any())
            {
                return NotFound("No orders detected");

            }

            var orderDto=  _mapper.Map<IEnumerable<OrderDto>>(order);

            return Ok(orderDto);
        
        }


        [HttpGet("{id:int}")]
        public async Task <ActionResult<OrderDto>> GetOrder(int id)
        {

            var order =await orderInterface.FindByIdAsync(id);

            if(order is not null)
            {
                var orderDto = _mapper.Map<OrderDto>(order);

                return orderDto is not null ? Ok(orderDto) : NotFound("Order not found");
            }

            return NotFound("Order not Found");



        }


        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<OrderDto>> GetClientsOrders(int clientId)
        {

            if (clientId <= 0)
                return BadRequest("Invalid data");

            var orders = await orderService.GetOrdersByClientId(clientId); 

            return !orders.Any()?NotFound("No Orders Found"):Ok(orders);

        }

        [HttpGet("details/{orderId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<OrderDetailsDto>> GetOrdersDetails(int orderId)
        {

            if (orderId <= 0)
                return BadRequest("Invalid data");

            var orderDetails=await orderService.GetOrderDetails(orderId);
            return orderDetails is not null ? Ok(orderDetails) : NotFound("No order where found");
        }


        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDto orderDto)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest("Incomplete data");

            }

            var order = _mapper.Map<Order>(orderDto);

            if(order is not null)
            {
                var response =await orderInterface.CreateAsync(order);

                return response.Flag?Ok(response): BadRequest("Order cant be placed");
            }

            return BadRequest("Order cant be placed");
        }



        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDto orderDto)

        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Please enter a valid order");
            }

            var order = _mapper.Map<Order>(orderDto);
            
            if( order is not null)
            {
                var respone=await orderInterface.UpdateAsync(order);

                return respone.Flag ? Ok(respone) : BadRequest("order cant be updated");
            }

            return BadRequest("order cant be updated");
        }


        [HttpDelete]

        public async Task<ActionResult<Response>> DeleteOrder (OrderDto orderDto)
        {


            if( !ModelState.IsValid)
            {
                return BadRequest("Failed to convert order");
            }
            var order = _mapper.Map<Order>(orderDto);
            if(order is not null)
            {
                var response = await orderInterface.DeleteAsync(order);
                return response.Flag ? Ok(response) : BadRequest("Failed to delete this order");
            }


            return BadRequest("Failed to delete this order");
        }

    }
}
