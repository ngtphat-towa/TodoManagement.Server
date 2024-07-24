using Application.Interfaces.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Shared.Services;

namespace Shared;

public static class RegisterShared
{
    public static IServiceCollection AddShared(this IServiceCollection services, ILogger logger)
    {
        try
        {
            services.AddScoped<IDateTimeService, DateTimeService>();
            services.AddScoped<IEmailService, EmailService>();

            logger.LogInformation($"{nameof(Shared)} layer initialized successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error initializing  {nameof(Shared)} layer: {ex.Message}",ex);
        }
        return services;
    }
}
