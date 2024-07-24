using Microsoft.OpenApi.Models;

namespace WebApi
{
    public static class RegisterPresentation
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, ILogger logger)
        {
            try
            {
                services.AddControllers();
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen(options =>
                {
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
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
                            new string[] {}
                        }
                    });
                });

                services.AddHttpContextAccessor();
                services.AddCors(options =>
                {
                    options.AddPolicy("AllowOrigin",
                        builder => builder.WithOrigins("http://localhost:4200")
                                          .AllowAnyMethod()
                                          .AllowAnyHeader());
                });
                logger.LogInformation($"{nameof(WebApi)} layer initialized successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error initializing {nameof(WebApi)} layer: {ex.Message}");
            }

            return services;
        }
    }
}
