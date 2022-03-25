using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Octopus.Conductor.Application.Constants;
using Octopus.Conductor.Application.Enums;
using Octopus.Conductor.Application.Exceptions;
using Octopus.Conductor.Infrastructure.WorkerService.Config;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.WorkerService.Abstractions
{
    public abstract class WorkerServiceBase : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly WorkerSettings _settings;
        private readonly IServiceProvider _serviceProvider;
        private Task _executingTask;
        private CancellationTokenSource _stoppingCts;

        public WorkerServiseStatus Status { get; private set; }

        public WorkerServiceBase(ILogger logger, WorkerSettings settings, IServiceProvider serviceProvider)
        {
            if (settings.RepeatIntervalSeconds <= 0)
                throw new IncorrectRepeatIntervalException("Incorrect repeat interval in seconds for worker service");

            _logger = logger;
            _settings = settings;
            _serviceProvider = serviceProvider;
            Status = WorkerServiseStatus.Created;
        }

        public abstract Task DoWorkAsync(IServiceScope scope, CancellationToken stoppingToken);
        public abstract void ExceptionHandle(Exception exception);
        protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();

                _logger.LogInformation("Background service is started");
                Status = WorkerServiseStatus.Running;

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await DoWorkAsync(scope, stoppingToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandle(ex);
                    }
                    await Task.Delay(1000 * _settings.RepeatIntervalSeconds, stoppingToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex,
                    "Execution Canceled",
                    stoppingToken.IsCancellationRequested);
                Status = WorkerServiseStatus.Stopped;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception. Execution Stopping");
                Status = WorkerServiseStatus.Faulted;
            }

            Status = WorkerServiseStatus.Completed;
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!IsWorkerRunning())
            {
                _stoppingCts = new CancellationTokenSource();
                _executingTask = ExecuteAsync(_stoppingCts.Token);
            }

            if (_executingTask.IsCompleted)
            {
                await _executingTask;
            }

            await Task.CompletedTask;
        }

        private bool IsWorkerRunning() => Status == WorkerServiseStatus.Running;

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
                await _executingTask;
            }
        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
        }
    }
}
