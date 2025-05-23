
using ProductApi.Infrastructure.DependencyInjection;

namespace ProductApi.Persentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddInfraStructureService(builder.Configuration);

            var app = builder.Build();
            app.UseInfraStructurePolicy();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

           


            app.MapControllers();

            app.Run();
        }
    }
}
