using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.WorkerService.Abstractions
{
    public abstract class WorkerServiceBase : IExtendedHostedService, IDisposable
    {
        private Task _executingTask;
        private CancellationTokenSource _stoppingCts;
        private ILogger _logger;

        public int RepeatIntervalSeconds { get; set; } = 1000;

        public WorkerServiceBase(ILogger logger)
        {
            _logger = logger;
        }

        public abstract Task DoWorkAsync(CancellationToken stoppingToken);
        public abstract void ExceptionHandle(Exception exception);
        protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Background service is started");
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await DoWorkAsync(stoppingToken).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandle(ex);
                    }
                    await Task.Delay(RepeatIntervalSeconds, stoppingToken).ConfigureAwait(false);
                }

                _logger.LogInformation(
                    "Background service is stoped",
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
            if (_executingTask == null ||
                _executingTask.Status == TaskStatus.RanToCompletion)
            {
                _stoppingCts = new CancellationTokenSource();
                _executingTask = DoWorkAsync(_stoppingCts.Token);
            }

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
                _executingTask.Dispose();
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
