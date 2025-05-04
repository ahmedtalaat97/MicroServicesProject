using eCommerce.SharedLibarary.MiddleWare;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.SharedLibarary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedService<TContext>(this IServiceCollection services,IConfiguration config,string fileName) where TContext : DbContext
        {
            //add Generic DB context



            services.AddDbContext<TContext>(option=>option.UseSqlServer(
                config.GetConnectionString("eCommerceConnection"),sqlServerOptions=>sqlServerOptions.EnableRetryOnFailure()));


            // config seriLog logging

            Log.Logger=new LoggerConfiguration().MinimumLevel.Information().WriteTo.Debug().WriteTo.Console()
                .WriteTo.File(path:$"{fileName}-.text",restrictedToMinimumLevel:Serilog.Events.LogEventLevel.Information,outputTemplate:"{Timestamp:yyyy-MM-dd HH:mm:ss.ff zzz}[{Level:u3}] {message:lj} {NewLine} {Exception}",rollingInterval:RollingInterval.Day)
                .CreateLogger();

            // Add JWT Auth Scheme

            JwtAuthenticationScheme.AddJWTAuthenticationScheme(services,config);

            return services;

        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalException>();

            app.UseMiddleware<ListenToOnlyApiGateway>();
            return app;
        }
    }
}
