using eCommerce.SharedLibarary.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interface;
using OrderApi.Application.Services;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.DependencyInjection
{
    public static class ServerContainer
    {

        public static IServiceCollection AddApplicationService(this IServiceCollection services,IConfiguration config)
        {

            // register el httpclient wl polly w imapper

            services.AddHttpClient<IOrderService, OrderService>(opt =>
            {
                opt.BaseAddress = new Uri(config["ApiGateway:BaseAddress"]!);
                opt.Timeout = TimeSpan.FromSeconds(1);
            });

            // create retry strategy

            var retryStrategy = new RetryStrategyOptions()
            {
                ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
                BackoffType = DelayBackoffType.Constant,
                UseJitter = true,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(500),
                OnRetry = args =>
                {
                    string message = $"OnRetry , Attempt:{args.AttemptNumber} OutCome {args.Outcome}";
                    LogException.LogToConsole(message);
                    LogException.LogToDebugger(message);
                    return ValueTask.CompletedTask;
                }

            };

            services.AddResiliencePipeline("my-retry-pipeline", builder =>
            {
                builder.AddRetry(retryStrategy);
            });
          

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;


        }


    }
}
