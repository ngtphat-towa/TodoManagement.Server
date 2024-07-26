using System.Reflection;

using Application.Behaviors;
using Application.Mapping;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application
{
    public static class RegisterApplication
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, ILogger logger)
        {
            try
            {
                services.AddScoped(
                        typeof(IPipelineBehavior<,>),
                        typeof(ValidationBehavior<,>));

                services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

                services.AddMappings();

                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterApplication).Assembly));


                logger.LogInformation($"{nameof(Application)} layer initialized successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error initializing {nameof(Application)} layer: {ex.Message}", ex);
            }


            return services;
        }
    }
}
