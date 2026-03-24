using System.Collections.Concurrent;
using System.Security.Claims;
using FlexFit.Domain.MongoModels.Models;
using FlexFit.Domain.MongoModels.Repositories;
using Microsoft.AspNetCore.Http;

namespace FlexFit.Presentation.Middleware
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
            var user = context.User;
            if (user.Identity?.IsAuthenticated == true && (user.IsInRole("Admin") || user.IsInRole("Employee")))
            {
                await _next(context);
                return;
            }

            var path = context.Request.Path.ToString().ToLower();
            var normalizedPath = GetNormalizedPath(path);

            if (!_limits.TryGetValue(normalizedPath, out var limit))
            {
                await _next(context);
                return;
            }

            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var key = userId != null ? $"user:{userId}:{normalizedPath}" : $"ip:{ip}:{normalizedPath}";

            if (IsBlocked(key))
            {
                await SendTooManyRequestsResponse(context, "Pristup privremeno blokiran zbog previse zahteva. Cekajte 15 minuta.");
                return;
            }

            var requests = RequestLog.GetOrAdd(key, _ => new List<DateTime>());
            lock (requests)
            {
                requests.RemoveAll(t => DateTime.UtcNow - t >= limit.Period);
                
                if (requests.Count >= limit.Limit)
                {
                    BlockedKeys[key] = DateTime.UtcNow.AddMinutes(15);
                    LogViolation(context, ip, userId, path);
                    SendTooManyRequestsResponse(context, "Previse zahteva. Pristup blokiran na 15 minuta.").Wait();
                    return;
                }
                
                requests.Add(DateTime.UtcNow);
                context.Response.Headers["X-RateLimit-Remaining"] = (limit.Limit - requests.Count).ToString();
            }

            await _next(context);
        }

        private string GetNormalizedPath(string path) => 
            path.StartsWith("/api/membershipcards/check-code") ? "/api/membershipcards/check-code" : path;

        private bool IsBlocked(string key)
        {
            if (BlockedKeys.TryGetValue(key, out var blockUntil))
            {
                if (DateTime.UtcNow < blockUntil) return true;
                BlockedKeys.TryRemove(key, out _);
            }
            return false;
        }

        private async Task SendTooManyRequestsResponse(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync($"{{\"message\": \"{message}\"}}");
        }

        private void LogViolation(HttpContext context, string ip, string? userId, string path)
        {
            var repo = context.RequestServices.GetRequiredService<RateLimitViolationRepository>();
            _ = repo.AddAsync(new RateLimitViolation 
            { 
                IpAddress = ip, 
                UserId = userId,
                Route = path, 
                Timestamp = DateTime.UtcNow 
            });
        }

    }
}