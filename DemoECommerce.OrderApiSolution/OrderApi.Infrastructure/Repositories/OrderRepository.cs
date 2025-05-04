using eCommerce.SharedLibarary.Logs;
using eCommerce.SharedLibarary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interface;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext _context) : IOrder
    {
        public async Task<Response> CreateAsync(Order entity)
        {
            try
            {

               var order= _context.Orders.Add(entity).Entity;

                await _context.SaveChangesAsync();

                return order.Id > 0 ? new Response(true, $"Order of id {order.Id} placed Successfully") : new Response(false, "Order can not be created");


            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);
                return new Response(false,"Error Occurred while creating order");
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order=await FindByIdAsync(entity.Id);

                if (order is null)
                    return new Response(false,"order not found");

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return new Response(true, $"Order deleted successfully deleted");



            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);
                return new Response(false, "Error Occurred while creating order");
            }
        }

        public async Task<Order> FindByIdAsync(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);

                return order is not null ? order : null!;



            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);
                throw new Exception("Error occured while retriving order");
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {

                var orders = await _context.Orders.AsNoTracking().ToListAsync();

                return orders is not null ?orders:null!;


            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);
                 throw new Exception( "Error Occurred while getting orders");
            }
        }

        public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {

                var order = await _context.Orders.Where(predicate).FirstOrDefaultAsync();
                return order is not null ? order : null!; 

            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);
                throw new Exception("Error Occurred while getting orders");
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var orders =await _context.Orders.Where(predicate).ToListAsync();

                return orders is not null ? orders : null !;    


            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);
                throw new Exception("Error Occurred while getting orders");
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
               var order=await FindByIdAsync(entity.Id);
                if(order != null)
                {
                    order.OrderDate = entity.OrderDate;
                    order.ProductId= entity.ProductId;
                    order.PurchaseQuantity= entity.PurchaseQuantity;
                    order.ClientId= entity.ClientId;
                    order.Id= entity.Id;

                    await _context.SaveChangesAsync();

                    return new Response(true, $"order of id {order.Id} updated successfully");
                   
                }

                return new Response(false,"order cannot be updated");

            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);
                return new Response(false, "Error Occurred while creating order");
            }
        }
    }
}
