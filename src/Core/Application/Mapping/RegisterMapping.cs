using System.Reflection;

using Mapster;

using MapsterMapper;

using Microsoft.Extensions.DependencyInjection;

namespace Application.Mapping
{
    public static class RegisterMapping
    {
        public static IServiceCollection AddMappings(this IServiceCollection services)
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());

            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
            return services;
        }
    }
}
