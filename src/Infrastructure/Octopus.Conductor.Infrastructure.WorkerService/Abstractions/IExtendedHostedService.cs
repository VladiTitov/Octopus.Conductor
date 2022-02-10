using Microsoft.Extensions.Hosting;
using Octopus.Conductor.Application.Enums;

namespace Octopus.Conductor.Infrastructure.WorkerService.Abstractions
{
    public interface IExtendedHostedService : IHostedService
    {
        public WorkerServiseStatus GetWorkerStatus();
    }
}
