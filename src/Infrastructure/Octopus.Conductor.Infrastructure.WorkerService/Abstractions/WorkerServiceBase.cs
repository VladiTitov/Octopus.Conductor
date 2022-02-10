﻿using Microsoft.Extensions.Logging;
using Octopus.Conductor.Application.Constants;
using Octopus.Conductor.Application.Enums;
using Octopus.Conductor.Application.Exceptions;
using Octopus.Conductor.Infrastructure.WorkerService.Config;
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
        private readonly WorkerSettings _settings;
        private WorkerServiseStatus _status;

        public WorkerServiceBase(ILogger logger, WorkerSettings settings)
        {
            if (settings.RepeatIntervalSeconds <= 0)
                throw new IncorrectRepeatIntervalException(ErrorMessages.IncorrectRepeatInterval);

            _logger = logger;
            _settings = settings;
            _status = WorkerServiseStatus.Created;
        }

        public abstract Task DoWorkAsync(CancellationToken stoppingToken);
        public abstract void ExceptionHandle(Exception exception);
        protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Background service is started");
                _status = WorkerServiseStatus.Running;

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await DoWorkAsync(stoppingToken).ConfigureAwait(false);
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
                _status = WorkerServiseStatus.Stoped;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception. Execution Stopping");
                _status = WorkerServiseStatus.Faulted;
            }

            _status = WorkerServiseStatus.Completed;
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            if (!IsWorkerRunning())
            {
                _stoppingCts = new CancellationTokenSource();
                _executingTask = ExecuteAsync(_stoppingCts.Token);
            }

            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            return Task.CompletedTask;
        }

        private bool IsWorkerRunning() => _status == WorkerServiseStatus.Running;

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

        public WorkerServiseStatus GetWorkerStatus()
        {
            return _status;
        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
        }
    }
}
