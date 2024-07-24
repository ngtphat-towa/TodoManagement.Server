using System.IdentityModel.Tokens.Jwt;
using System.Text;

using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

using Domain.Settings;

using Identity.Context;
using Identity.Models;
using Identity.Seeds;
using Identity.Services;

using Infrastructure.Persistence.Repositories;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

using Shared.Wrappers;

namespace Identity
{
    public static class RegisterIdentity
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            ConfigureDatabase(services, configuration, logger);
            ConfigureIdentity(services);
            ConfigureAuthentication(services, configuration);

            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IUserService, UserService>();
            services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
            services.AddScoped<ITokenService, TokenService>();

            logger.LogInformation($"{nameof(Identity)} layer initialized successfully.");

            return services;
        }

        private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            var isMemoryData = configuration.GetValue<bool>("UseInMemoryDatabase");

            logger.LogDebug($"UseInMemoryDatabase = {isMemoryData} .");
            if (isMemoryData)
            {
                services.AddDbContext<IdentityContext>(options =>
                    options.UseInMemoryDatabase("IdentityDb"));
            }
            else
            {
                services.AddDbContext<IdentityContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));
            }
        }

        private static void ConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();
        }

        private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JWTSettings"));
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    var jwtSettings = configuration.GetSection("JWTSettings").Get<JwtSettings>();
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = jwtSettings?.Issuer,
                        ValidAudience = jwtSettings?.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                        ClockSkew = TimeSpan.Zero,
                        RequireExpirationTime = true
                    };
                    o.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = c =>
                        {
                            if (c.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                c.Response.Headers.Append("Token-Expired", "true");
                            }
                            c.NoResult();
                            c.Response.StatusCode = 500;
                            c.Response.ContentType = "text/plain";
                            return c.Response.WriteAsync(c.Exception.ToString());
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Response<bool>.Failure("You are not Authorized"));
                            return context.Response.WriteAsync(result);
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Response<bool>.Failure("You are not authorized to access this resource"));
                            return context.Response.WriteAsync(result);
                        },

                    };
                });
        }
        public static async Task SeedIdentityData(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            var isMemoryData = configuration.GetValue<bool>("UseInMemoryDatabase");
            if (isMemoryData)
            {
                // Retrieve the necessary services
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                    // Ensure roles and users are seeded
                    await DefaultRoles.SeedAsync(userManager, roleManager);
                }
                logger.LogInformation($"Info: Seeding {nameof(Identity)} data successfully.");
            }

        }
    }
}
