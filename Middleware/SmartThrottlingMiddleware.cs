using System.Collections.Concurrent;
using System.Security.Claims;
using FlexFit.MongoModels.Models;
using FlexFit.MongoModels.Repositories;
using Microsoft.AspNetCore.Http;

namespace FlexFit.Middleware
{
    public class SmartThrottlingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, List<DateTime>> RequestLog = new();
        private static readonly ConcurrentDictionary<string, DateTime> BlockedKeys = new();

        // Limit definitions based on requirements
        private readonly Dictionary<string, (int Limit, TimeSpan Period)> _limits = new()
        {
            { "/api/fitnessobjects", (30, TimeSpan.FromMinutes(1)) },
            { "/api/membershipcards/check-code", (3, TimeSpan.FromMinutes(1)) },
            { "/api/guard/log-entry", (60, TimeSpan.FromMinutes(1)) }
        };

        public SmartThrottlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLower();
            
            // Normalize path for parametrized routes
            string normalizedPath = path;
            if (path.StartsWith("/api/membershipcards/check-code"))
            {
                normalizedPath = "/api/membershipcards/check-code";
            }

            if (!_limits.TryGetValue(normalizedPath, out var limit))
            {
                await _next(context);
                return;
            }

            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // Track by User ID if authenticated, otherwise fallback to IP
            var key = userId != null ? $"user:{userId}:{normalizedPath}" : $"ip:{ip}:{normalizedPath}";

            // Check if blocked
            if (BlockedKeys.TryGetValue(key, out var blockUntil))
            {
                if (DateTime.UtcNow < blockUntil)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"message\": \"Pristup privremeno blokiran zbog previše zahteva. Sačekajte 15 minuta.\"}");
                    return;
                }
                else
                {
                    BlockedKeys.TryRemove(key, out _);
                }
            }

            var list = RequestLog.GetOrAdd(key, _ => new List<DateTime>());

            lock (list)
            {
                list.RemoveAll(t => DateTime.UtcNow - t >= limit.Period);
                
                // Add header info
                int remaining = limit.Limit - list.Count;
                context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, remaining - 1).ToString();

                if (list.Count >= limit.Limit)
                {
                    // Block for 15 minutes
                    BlockedKeys[key] = DateTime.UtcNow.AddMinutes(15);

                    // Log violation to MongoDB
                    _ = context.RequestServices.GetRequiredService<RateLimitViolationRepository>()
                        .AddAsync(new RateLimitViolation 
                        { 
                            IpAddress = ip, 
                            UserId = userId,
                            Route = path, 
                            Timestamp = DateTime.UtcNow 
                        });

                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.ContentType = "application/json";
                    context.Response.WriteAsync("{\"message\": \"Previše zahteva. Pristup blokiran na 15 minuta.\"}").Wait();
                    return;
                }
                list.Add(DateTime.UtcNow);
            }
            await _next(context);
        }
    }
}