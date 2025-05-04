using eCommerce.SharedLibarary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfraStructureService(this IServiceCollection services, IConfiguration config)
        {

            SharedServiceContainer.AddSharedService<ProductDbContext>(services, config, config["MySeriLog:FileName"]!);


            services.AddScoped<IProduct, ProductRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfraStructurePolicy(this IApplicationBuilder app)
        {

            SharedServiceContainer.UseSharedPolicies(app);

            return app;

        }
    }
}
