using Microsoft.Extensions.DependencyInjection;
using Octopus.Conductor.Application.Interfaces;
using Octopus.Conductor.Application.Services;

namespace Octopus.Conductor.Application
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IFolderListener, FolderListener>();
        }
    }
}
