using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.Hosting
{
    public interface IExtendedHostingService : IHostedService
    {
        public TaskStatus GetWorkerStatus();
    }
}
