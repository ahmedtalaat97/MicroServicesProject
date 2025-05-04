using Microsoft.EntityFrameworkCore;
using OrderApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Infrastructure.Context
{
    public class OrderDbContext:DbContext
    {

        public OrderDbContext(DbContextOptions<OrderDbContext> options ):base(options) 
        {
        
            
        
        }
        public DbSet<Order> Orders { get; set; }    
       
    }
}
