using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.WorkerService.Abstractions
{
    public interface IExtendedHostedService : IHostedService
    {
        public TaskStatus GetWorkerStatus();
    }
}
