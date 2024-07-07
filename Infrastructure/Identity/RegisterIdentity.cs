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
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

using Shared.Wrappers;

namespace Identity
{
    public static class RegisterIdentity
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureDatabase(services, configuration);
            ConfigureIdentity(services);
            ConfigureAuthentication(services, configuration);
            SeedIdentityData(services);

            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IUserService, UserService>();
            services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();

            Console.WriteLine($"Info: {nameof(Identity)} layer initialized successfully.");

            return services;
        }

        private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
        {
            var isMemoryData = configuration.GetValue<bool>("UseInMemoryDatabase");
            Console.WriteLine($"Data-Info: UseInMemoryDatabase = {isMemoryData} .");
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
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]!))
                };
                o.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = c =>
                    {
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
        private static void SeedIdentityData(IServiceCollection services)
        {
            // Retrieve the necessary services
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Ensure roles and users are seeded
                DefaultRoles.SeedAsync(userManager, roleManager).Wait();
            }
        }
    }
}
