using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace SalesAnalysisApp
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {

                _logger.LogError("Error Occured {message}", ex.Message);
                await HandleExceptionAsync(httpContext, ex,HttpStatusCode.BadRequest);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode )
        {
          
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                error = ex.Message,
                statusCode = (int)statusCode,
                timestamp = DateTime.UtcNow
            };
            
           // var jsonResponse=JsonConvert.SerializeObject(response);

            await context.Response.WriteAsJsonAsync(response);


            
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
