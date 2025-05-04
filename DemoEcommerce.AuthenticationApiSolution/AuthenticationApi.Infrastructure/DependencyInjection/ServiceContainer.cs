using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Context;
using AuthenticationApi.Infrastructure.Repositories;
using eCommerce.SharedLibarary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services,IConfiguration config)
        {
            //Add database, Jwt Auth scheme

            SharedServiceContainer.AddSharedService<AuthenticationDbContext>(services, config, config["MySeriLog:FileName"]!);


            //DI
            services.AddScoped<IUser,UserRepository> ();



            return services;
        }

        public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Global Exc , Listen only to api gateway

            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}
