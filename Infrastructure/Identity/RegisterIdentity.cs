using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity;

public static class RegisterIdentity
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        var isMemoryData = configuration.GetValue<bool>("UseInMemoryDatabase");
        try
        {

            Console.WriteLine($"Info: {nameof(Identity)} layer initialized successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing persistence layer: {ex.Message}");
        }

        return services;
    }
}