using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Octopus.Conductor.Core.Interfaces;
using Octopus.Conductor.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(
                    databaseName: "InMemory"));

        public static void AddRepositories(this IServiceCollection services) =>
            services.AddScoped<IRepository, EfRepository>();
    }
}
