using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octopus.Conductor.Core.Entities;
using Octopus.Conductor.Core.Interfaces;
using Octopus.Conductor.WebApi.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Conductor.WebApi.Services
{
    //TODO Resolve issue with second instance start
    public class FileMovingBackgroundService : BackgroundService
    {

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly WorkerSettings _settings;

        public FileMovingBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<FileMovingBackgroundService> logger,
            IOptions<WorkerSettings> settings)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("HostedService is started");
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        _logger.LogInformation($"{DateTime.Now}");
                        await DoWorkAsync(stoppingToken).ConfigureAwait(false);
                    }
                    catch (AggregateException ae)
                    {
                        foreach (var ex in ae.InnerExceptions)
                            _logger.LogError(
                            ex,
                            ex.Message,
                            GetType());
                    }
                    catch (OperationCanceledException ex)
                    {
                        _logger.LogWarning(ex, "Execution Canceled");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            ex.Message,
                            GetType());
                    }
                    await Task.Delay(_settings.DelaySeconds * 1000, stoppingToken).ConfigureAwait(false);
                }

                _logger.LogInformation(
                    "HostedService is stoped",
                    stoppingToken.IsCancellationRequested);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "Execution Canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception. Execution Stopping");
            }
        }

        private async Task DoWorkAsync(CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var folderListener = scope.ServiceProvider.GetRequiredService<IFolderListener>();
                await folderListener.MoveEntityFilesAsync(cancellationToken);
            }
        }
    }
}
