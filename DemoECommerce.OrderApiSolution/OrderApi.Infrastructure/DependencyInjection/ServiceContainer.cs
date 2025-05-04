using eCommerce.SharedLibarary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interface;
using OrderApi.Infrastructure.Context;
using OrderApi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {

        public static IServiceCollection AddInfraStructureService(this IServiceCollection services, IConfiguration config)
        {

            SharedServiceContainer.AddSharedService<OrderDbContext>(services, config, config["MySerilog:FileName"]!);

            services.AddScoped<IOrder,OrderRepository>();

            return services;

        }
        public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
        {

            SharedServiceContainer.UseSharedPolicies(app);

            return app;
        }
    }
}
