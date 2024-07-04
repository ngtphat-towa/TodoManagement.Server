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
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterApplication).Assembly));

            services.AddScoped(
                    typeof(IPipelineBehavior<,>),
                    typeof(ValidationBehavior<,>));

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMappings();

            return services;
        }
    }
}
