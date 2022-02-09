using System;

namespace Octopus.Conductor.Infrastructure.WorkerService.Exceptions
{
    public class IncorrectRepeatInterval : Exception
    {
        public IncorrectRepeatInterval(string msg): base(msg) { }
    }
}
