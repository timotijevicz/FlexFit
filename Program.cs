using System.Reflection;
using System.Text;
using FlexFit.Infrastructure.Data;
using FlexFit.Presentation.Middleware;
using FlexFit.Domain.MongoModels.Repositories;
using FlexFit.Infrastructure.Token;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using FlexFit.Infrastructure.Repositories.Interfaces;
using FlexFit.Infrastructure.Repositories;
using FlexFit.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace FlexFit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpContextAccessor();

            // PostgreSQL
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // MongoDB
            builder.Services.AddSingleton<MongoDbContext>();

            // Neo4j
            builder.Services.AddSingleton<Neo4jContext>();

            // Unit of Work + repositories
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<EntryLogRepository>();
            builder.Services.AddScoped<LoginRepository>();
            builder.Services.AddScoped<IncidentRepository>();
            builder.Services.AddScoped<RateLimitViolationRepository>();
            builder.Services.AddScoped<FlexFit.Infrastructure.Repositories.Interfaces.ITimeSlotRepository, FlexFit.Infrastructure.Repositories.TimeSlotRepository>();
            builder.Services.AddScoped<FlexFit.Infrastructure.Repositories.Interfaces.IResourceRepository, FlexFit.Infrastructure.Repositories.ResourceRepository>();
            builder.Services.AddScoped<ReservationLogRepository>();
            builder.Services.AddScoped<PenaltyLogRepository>();
            builder.Services.AddScoped<MembershipLogRepository>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
            builder.Services.AddScoped<IPenaltyCardRepository, PenaltyCardRepository>();
            builder.Services.AddScoped<IPenaltyPointRepository, PenaltyPointRepository>();
            builder.Services.AddScoped<IMemberGraphRepository, MemberGraphRepository>();
            builder.Services.AddHostedService<FlexFit.Application.Services.ReservationBackgroundService>();


            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });
            // MediatR
            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Token service
            builder.Services.AddScoped<ITokenService, TokenService>();

            // Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            })
            .AddCookie()
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
                googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
            });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:5175")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Controllers + Swagger
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Unesi token u formatu: Bearer {token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // Global exception handler
            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            // Custom throttling middleware
            app.UseMiddleware<SmartThrottlingMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}