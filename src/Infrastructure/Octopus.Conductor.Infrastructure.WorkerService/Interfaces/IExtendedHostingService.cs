using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.WorkerService.Interfaces
{
    public interface IExtendedHostingService : IHostedService
    {
        public TaskStatus GetWorkerStatus();
    }
}
