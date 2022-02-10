using System;

namespace Octopus.Conductor.Infrastructure.WorkerService.Exceptions
{
    public class IncorrectRepeatIntervalException : Exception
    {
        public IncorrectRepeatIntervalException(string msg): base(msg) { }
    }
}
