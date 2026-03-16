using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using FlexFit.MongoModels.Repositories;

namespace FlexFit.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try { await _next(context); }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška: {ex.Message}");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new
                {
                    Error = "Server Error",
                    Details = ex.Message
                });
            }
        
    }
    
    }
}