using Octopus.Conductor.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Octopus.Conductor.Core.Interfaces
{
    public interface IFolderListener
    {
        public Task MoveEntityFilesAsync(IEnumerable<ConductorEntityDescription> descriptions);
    }
}
