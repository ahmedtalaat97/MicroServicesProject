using eCommerce.SharedLibarary.Logs;
using eCommerce.SharedLibarary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext _context) : IProduct

    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                var getProduct=await GetByAsync(_=>_.Name!.Equals(entity.Name));

                if(getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                {
                    return new Response(false, $"{entity.Name} already exist");
                }

                var currentEntity=_context.Products.Add(entity).Entity;
                await _context.SaveChangesAsync();
                if(currentEntity is not null&&currentEntity.Id>0)
                {
                    return new Response(true, $"{entity.Name} added to database successfully");
                }
                else
                {
                    return new Response(false, $"Error adding {entity.Name} ");
                }

            }
            catch (Exception ex)
            {

                //Log the original excep
                LogException.LogExceptions(ex);

                // nzhar message lel client

                return new Response(false, "Error Occurred adding new Product");
            }   
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product=await FindByIdAsync(entity.Id);
                if(product is null)
                {
                    return new Response(false, $"{entity.Name} is not found");

                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return new Response(true, $"{entity.Name} is deleted successfully");


            }
            catch (Exception ex)
            {
                //Log the original excep
                LogException.LogExceptions(ex);

                // nzhar message lel client

                return new Response(false, "Error Occurred adding new Product");

            }

        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product= await _context.Products.FindAsync(id);

                if(product is null)
                {
                    return null;
                }
                else
                {
                    return product;
                }

            }
            catch (Exception ex)
            {

                //Log the original excep
                LogException.LogExceptions(ex);

                // nzhar message lel client

                throw new Exception( "Error Occurred adding new Product");

            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await _context.Products.AsNoTracking().ToListAsync();

                return products is not null?products:null!;

            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);

                throw new InvalidOperationException("Error occurred retrieving products");
            }


        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {

                var product = await _context.Products.Where(predicate).FirstOrDefaultAsync();
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);

                throw new InvalidOperationException("Error occurred retrieving products");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product=await FindByIdAsync(entity.Id);
                if (product is null)
                {
                    return new Response(false, $"{entity.Name} is not found");
                }

                product.Name = entity.Name;
                product.Price = entity.Price;
                product.Quantity = entity.Quantity;
               
                // add more fields as needed

                await _context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} is updated successfully");
            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);

                // nzhar message lel client

                return new Response(false, "Error Occurred updating this Product");
            }
        }
    }
}
