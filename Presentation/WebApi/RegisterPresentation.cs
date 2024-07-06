using Application.Interfaces.Services;

using WebApi.Services;

namespace WebApi;

public static class RegisterPresentation
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        try
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddHttpContextAccessor();

            services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();

            Console.WriteLine($"Info: {nameof(WebApi)} layer initialized successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing  {nameof(WebApi)} layer: {ex.Message}");
        }
       

        return services;
    }
}
