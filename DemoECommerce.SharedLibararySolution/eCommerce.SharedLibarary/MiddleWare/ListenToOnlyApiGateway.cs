using Azure.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.SharedLibarary.MiddleWare
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {

        public async Task InvokeAsync(HttpContext context)
        {
            // hatl3 el header mn el request 

            var signedHeader = context.Request.Headers["Api-Gateway"];

            // law null => request is not coming from api Gateway

            if(signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

                await context.Response.WriteAsync("Sorry, service is unavailable");
                return;
            }else
            {
                await next(context);
            }
        }
    }
}
