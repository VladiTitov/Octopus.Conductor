using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.WorkerService.Enums
{
    public enum WorkerServiseStatus
    {
        Created,
        Running,
        Stoped,
        Faulted,
        Completed
    }
}
