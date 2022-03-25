using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Octopus.Conductor.Application.Interfaces;
using Octopus.Conductor.Infrastructure.RelationalDB.Repositories;

namespace Octopus.Conductor.Infrastructure.RelationalDB
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {

            var a = configuration.GetConnectionString("SQLiteConnection");
            services.AddDbContext<EntitiesDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("SQLiteConnection")));
        }

        public static void AddRepositories(this IServiceCollection services) =>
            services.AddScoped<IRepository, EfRepository>();
    }
}
