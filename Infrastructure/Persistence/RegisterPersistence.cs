using Application.Interfaces.Common;
using Application.Interfaces.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Persistence.Context;
using Persistence.Repositories;

namespace Persistence;

public static class RegisterPersistence
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var isMemoryData = configuration.GetValue<bool>("UseInMemoryDatabase");
        Console.WriteLine(isMemoryData);
        if (isMemoryData)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("ApplicationDb"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
           options.UseSqlServer(
               configuration.GetConnectionString("DefaultConnection")
             ));
        }
        #region Repositories
        services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepository<>));
        services.AddTransient<ITodoRepository, TodoRepository>();
        #endregion
        return services;
    }
}
