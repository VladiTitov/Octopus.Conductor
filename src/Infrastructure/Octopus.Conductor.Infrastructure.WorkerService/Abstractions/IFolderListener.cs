using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.WorkerService.Abstractions
{
    public interface IFolderListener
    {
        Task MoveEntityFilesAsync(CancellationToken cancellationToken = default);
    }
}
