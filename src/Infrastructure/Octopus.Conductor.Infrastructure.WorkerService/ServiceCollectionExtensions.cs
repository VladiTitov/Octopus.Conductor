using Microsoft.Extensions.DependencyInjection;
using Octopus.Conductor.Infrastructure.WorkerService.Abstractions;
using Octopus.Conductor.Infrastructure.WorkerService.Services;

namespace Octopus.Conductor.Infrastructure.WorkerService
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWorkerServices(this IServiceCollection services)
        {
            services.AddSingleton<WorkerServiceBase, FileMovingWorkerService>();
        }

        public static void AddFolderListner(this IServiceCollection services)
        {
            services.AddScoped<IFolderListener, FolderListener>();
        }
    }
}
