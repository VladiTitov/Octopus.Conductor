using Octopus.Conductor.Core.Common;

namespace Octopus.Conductor.Core.Entities
{
    public class EntityDescription : BaseEntity
    {
        public string EntityType { get; set; }
        public string InputDirectory { get; set; }
        public string OutputDirectory { get; set; }
    }
}
