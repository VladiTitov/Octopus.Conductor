using System;

namespace Octopus.Conductor.Application.Exceptions
{
    public class IncorrectRepeatIntervalException : Exception
    {
        public IncorrectRepeatIntervalException(string msg): base(msg) { }
    }
}
