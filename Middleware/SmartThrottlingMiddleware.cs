using System.Collections.Concurrent;
using FlexFit.MongoModels;
using FlexFit.MongoModels.Models;
using FlexFit.MongoModels.Repositories;
using FlexFit.Repositories;
using Microsoft.AspNetCore.Http;

namespace FlexFit.Middleware
{
    public class SmartThrottlingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, List<DateTime>> RequestLog = new();

        // Limit za svaki tip rute
        private readonly Dictionary<string, (int Limit, TimeSpan Period)> _limits = new()
        {
            { "/api/member/check-availability", (30, TimeSpan.FromMinutes(1)) },
            { "/api/member/activate-card", (3, TimeSpan.FromMinutes(1)) },
            { "/api/employee/scan", (60, TimeSpan.FromMinutes(1)) }
        };

        public SmartThrottlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLower();
            if (!_limits.TryGetValue(path, out var limit)) { await _next(context); return; }

            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var key = $"{ip}:{path}";

            // GetOrAdd skraćuje kod - uzima listu ili pravi novu u jednoj liniji
            var list = RequestLog.GetOrAdd(key, _ => new List<DateTime>());

            lock (list)
            {
                list.RemoveAll(t => DateTime.UtcNow - t >= limit.Period);
                if (list.Count >= limit.Limit)
                {
                    // Logovanje u MongoDB (Fire-and-forget)
                    _ = context.RequestServices.GetRequiredService<RateLimitViolationRepository>()
                        .AddAsync(new RateLimitViolation { IpAddress = ip, Route = path, Timestamp = DateTime.UtcNow });

                    context.Response.StatusCode = 429;
                    context.Response.WriteAsync("Prebrzo skenirate! Sačekajte.").Wait();
                    return;
                }
                list.Add(DateTime.UtcNow);
            }
            await _next(context);
        }
    }
}