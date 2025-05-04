using ApiGateway.Persentation.MiddleWare;
using eCommerce.SharedLibarary.DependencyInjection;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ApiGateway.Persentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            
            builder.Configuration.AddJsonFile("ocelot.json",optional:false, reloadOnChange: true);
            builder.Services.AddOcelot().AddCacheManager(x=>x.WithDictionaryHandle());

            JwtAuthenticationScheme.AddJWTAuthenticationScheme(builder.Services,builder.Configuration);

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseCors();
            app.UseMiddleware<AttachSignatureToRequest>();
            app.UseOcelot().Wait();
            app.UseHttpsRedirection();

        

          

          

            app.Run();
        }
    }
}
