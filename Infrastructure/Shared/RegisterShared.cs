using Application.Interfaces.Services;

using Microsoft.Extensions.DependencyInjection;

using Shared.Services;

namespace Shared;

public static class RegisterShared
{
    public static IServiceCollection AddShared(this IServiceCollection services)
    {
        services.AddScoped<IDateTimeService, DateTimeService>();
        return services;
    }
}
