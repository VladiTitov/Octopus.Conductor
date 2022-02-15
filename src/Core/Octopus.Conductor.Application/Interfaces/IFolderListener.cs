using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Conductor.Application.Interfaces
{
    public interface IFolderListener
    {
        Task MoveEntityFilesAsync(CancellationToken cancellationToken = default);
    }
}
