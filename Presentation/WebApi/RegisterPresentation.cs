using Application.Interfaces.Services;

using WebApi.Services;

namespace WebApi;

public static class RegisterPresentation
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddHttpContextAccessor();

        services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
        return services;
    }
}
