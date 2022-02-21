using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octopus.Conductor.Application.Interfaces;
using Octopus.Conductor.Infrastructure.WorkerService.Abstractions;
using Octopus.Conductor.Infrastructure.WorkerService.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.WorkerService.Services
{
    public class FileMovingWorkerService : WorkerServiceBase
    {
        private readonly ILogger _logger;

        public FileMovingWorkerService(
            ILogger<FileMovingWorkerService> logger,
            IServiceProvider serviceProvider,
            IOptions<WorkerSettings> settings) : base(logger, settings.Value, serviceProvider)
        {
            _logger = logger;
        }
        public override async Task DoWorkAsync(IServiceScope scope, CancellationToken stoppingToken)
        {
            var folderListener = scope.ServiceProvider.GetRequiredService<IFolderListener>();
            await folderListener.MoveEntityFilesAsync(stoppingToken);

        }

        public override void ExceptionHandle(Exception exception)
        {
            switch (exception)
            {
                case AggregateException ae:
                    foreach (var ex in ae.InnerExceptions)
                        _logger.LogError(
                            ex,
                            ex.Message,
                            GetType().Name);
                    break;

                case Exception ex:
                    _logger.LogError(
                        ex,
                        "Unhandled exception",
                        GetType().Name);
                    break;

                default:
                    return;
            }
        }
    }
}
