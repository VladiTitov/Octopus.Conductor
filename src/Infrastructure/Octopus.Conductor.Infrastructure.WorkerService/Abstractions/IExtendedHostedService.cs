using Microsoft.Extensions.Hosting;
using Octopus.Conductor.Infrastructure.WorkerService.Enums;

namespace Octopus.Conductor.Infrastructure.WorkerService.Abstractions
{
    public interface IExtendedHostedService : IHostedService
    {
        public WorkerServiseStatus GetWorkerStatus();
    }
}
