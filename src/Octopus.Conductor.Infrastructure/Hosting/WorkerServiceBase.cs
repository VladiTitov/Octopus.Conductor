using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.Hosting
{
    public abstract class WorkerServiceBase : IExtendedHostingService, IDisposable
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private  ILogger _logger;

        public WorkerServiceBase(
            ILogger<WorkerServiceBase> logger)
        {
            _logger = logger;
        }

        public abstract Task DoWorkAsync(CancellationToken stoppingToken);
        public abstract void ExceptionHandle(Exception exception);
        
        protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
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
                    catch (Exception ex)
                    {
                        ExceptionHandle(ex);
                    }
                    await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
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
        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            if (_executingTask.Status == TaskStatus.Running)
                return Task.CompletedTask;

            _executingTask = ExecuteAsync(_stoppingCts.Token);

            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public TaskStatus GetWorkerStatus()
        {
            return _executingTask.Status;
        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
        }
    }
}
