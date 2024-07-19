using Application.Interfaces.Common;
using Application.Interfaces.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Persistence.Context;
using Persistence.Repositories;
using Persistence.Seeding;

using System;

namespace Persistence
{
    public static class RegisterPersistence
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var isMemoryData = configuration.GetValue<bool>("UseInMemoryDatabase");

            try
            {
                if (isMemoryData)
                {
                    AddInMemoryDatabase(services);
                    SeedInMemoryDatabase(services);
                }
                else
                {
                    AddSqlServerDatabase(services, configuration);
                }
                RegisterRepositories(services);

                Console.WriteLine($"Info: Memory data initialized successfully.");
                Console.WriteLine($"Info: {nameof(Persistence)} layer initialized successfully.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing {nameof(Persistence)} layer: {ex.Message}");
            }

            return services;
        }

        private static void AddInMemoryDatabase(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("ApplicationDb"));
        }

        private static void SeedInMemoryDatabase(IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                ApplicationDbContextSeed.Seed(dbContext);
            }
        }

        private static void AddSqlServerDatabase(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepository<>));
            services.AddTransient<ITodoRepository, TodoRepository>();


        }
    }
}
