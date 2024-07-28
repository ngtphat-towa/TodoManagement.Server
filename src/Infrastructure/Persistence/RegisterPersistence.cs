using Application.Interfaces.Common;
using Application.Interfaces.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Persistence.Context;
using Persistence.Repositories;
using Persistence.Seeding;

namespace Persistence
{
    public static class RegisterPersistence
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            try
            {
                if (configuration.GetValue<bool>("UseInMemoryDatabase"))
                {
                    AddInMemoryDatabase(services);
                }
                else
                {
                    AddSqlServerDatabase(services, configuration);
                }
                RegisterRepositories(services);

                logger.LogInformation($"Memory data initialized successfully.");
                logger.LogInformation($"{nameof(Persistence)} layer initialized successfully.");

            }
            catch (Exception ex)
            {
                logger.LogError($"Error initializing {nameof(Persistence)} layer: {ex.Message}", ex);
            }

            return services;
        }

        private static void AddInMemoryDatabase(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("ApplicationDb"));
        }

        public static async Task SeedPersistenceData(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            var isMemoryData = configuration.GetValue<bool>("UseInMemoryDatabase");
            if (isMemoryData)
            {
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                    await ApplicationDbContextSeed.Seed(dbContext);
                }
                logger.LogInformation($"Seeding {nameof(Persistence)} data successfully.");
            }
        }

        private static void AddSqlServerDatabase(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddTransient<ITodoRepository, TodoRepository>();

        }
    }
}
