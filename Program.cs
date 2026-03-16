// ... tvoji existing usings ...
using System.Reflection;
using System.Text;
using FlexFit.Data;
using FlexFit.Middleware;
using FlexFit.MongoModels.Repositories;
using FlexFit.Token;
using FlexFit.UnitOfWorkLayer;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FlexFit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Baze (PostgreSQL & MongoDB)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSingleton<MongoDbContext>();

            // 2. Registracija MediatR-a (Ovo ti je klju?no za Handler-e)
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // 3. UnitOfWork i Repozitorijumi
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Napomena: Pošto UnitOfWork inicijalizuje repozitorijume u konstruktoru, 
            // tehni?ki ne moraš svaki repo posebno registrovati, ali nije greška ako stoje.
            builder.Services.AddScoped<EntryLogRepository>();
            builder.Services.AddScoped<RateLimitViolationRepository>();

            // 4. Autentifikacija (JWT + Google)
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            })
            .AddCookie() // OBAVEZNO ZA GOOGLE LOGIN
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            });

            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
         

            builder.Services.AddSingleton<MongoDbContext>();
            builder.Services.AddScoped<EntryLogRepository>();
            builder.Services.AddScoped<IncidentRepository>();
            builder.Services.AddScoped<RateLimitViolationRepository>();

            // Registracija MediatR-a
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            var app = builder.Build();

            // 5. Middleware redosled (Ovo je važno!)
            // ExceptionMiddleware ide prvi da uhvati SVE greške
            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting(); // Routing ide pre Throttlinga i Autentifikacije

            // Throttling proverava brzinu pre nego što trošimo resurse na login
            app.UseMiddleware<SmartThrottlingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}