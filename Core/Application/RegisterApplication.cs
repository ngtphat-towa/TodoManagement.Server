using System.Reflection;

using Application.Behaviors;
using Application.Mapping;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class RegisterApplication
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            try
            {
                services.AddScoped(
                        typeof(IPipelineBehavior<,>),
                        typeof(ValidationBehavior<,>));

                services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

                services.AddMappings();

                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterApplication).Assembly));


                Console.WriteLine($"Info: {nameof(Application)} layer initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing {nameof(Application)} layer: {ex.Message}");
            }


            return services;
        }
    }
}
