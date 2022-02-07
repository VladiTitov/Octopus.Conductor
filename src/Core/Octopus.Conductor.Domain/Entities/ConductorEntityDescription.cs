using Octopus.Conductor.Domain.Common;

namespace Octopus.Conductor.Domain.Entities
{
    public class ConductorEntityDescription : BaseEntity
    {
        public string EntityType { get; set; }
        public string InputDirectory { get; set; }
        public string OutputDirectory { get; set; }
    }
}
