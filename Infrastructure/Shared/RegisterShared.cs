using Application.Interfaces.Services;

using Microsoft.Extensions.DependencyInjection;

using Shared.Services;

namespace Shared;

public static class RegisterShared
{
    public static IServiceCollection AddShared(this IServiceCollection services)
    {
        try
        {
            services.AddScoped<IDateTimeService, DateTimeService>();

            Console.WriteLine($"Info: {nameof(Shared)} layer initialized successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing  {nameof(Shared)} layer: {ex.Message}");
        }
        return services;
    }
}
