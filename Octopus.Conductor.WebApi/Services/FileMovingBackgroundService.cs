using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Octopus.Conductor.Core.Entities;
using Octopus.Conductor.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Conductor.WebApi.Services
{
    public class FileMovingBackgroundService : BackgroundService
    {

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public FileMovingBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<FileMovingBackgroundService> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("HostedService is Starting!");
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("HostedService is stopping!");
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("hh:mm:ss")} Working...");
                //using (var scope = _serviceProvider.CreateScope())
                //{
                //    var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
                //    var folderListener=scope.ServiceProvider.GetRequiredService<IFolderListener>();
                //    var descriptions=await repository.GetAllAsync<EntityDescription>();

                //    await folderListener.MoveFilesAsync(descriptions);
                //    await Task.Delay(5000);
                //}
                await Task.Delay(1000);
            }
        }
    }
}
