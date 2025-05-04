using eCommerce.SharedLibarary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eCommerce.SharedLibarary.MiddleWare
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string message = "Sorry , Internal Server error occurred .Try again or Contact Ahmed Talaat ";

            int statusCode=(int) HttpStatusCode.InternalServerError;

            string title = "Error";
            try
            {

                await next(context);
                // han3ml check law kan fy kaza request 429
                if(context.Response.StatusCode==StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "Too many Requests made";
                    statusCode = StatusCodes.Status429TooManyRequests;

                    await ModifyHeader(context,title,message,statusCode);

                }

                // Law msh ma3mlo Authorize 401
                if(context.Response.StatusCode==StatusCodes.Status401Unauthorized)
                {
                    title= "Alert";
                    message = "Please make sure you are authorized to access";
                    statusCode= StatusCodes.Status401Unauthorized;
                    await ModifyHeader(context,title,message, statusCode);
                }

                // law el response forbidden 403

                if(context.Response.StatusCode==StatusCodes.Status403Forbidden)
                {
                    title = "Out of Service";
                    message = "You are not allowed to access .";
                    statusCode=StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {

                // hanst5dm el logs bta3tna file ,debugger console
                LogException.LogExceptions(ex);

                // check law el exception timeout
                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of time";
                    message = "Request Time out";
                    statusCode=(int) StatusCodes.Status408RequestTimeout;

                 
                }
                await ModifyHeader(context, title, message, statusCode);
            }
        }

        private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail=message,
                Status=statusCode,
                Title=title
            }),CancellationToken.None);

            return;
          
        }
    }
}
