using Octopus.Conductor.Application.Interfaces;
using Octopus.Conductor.Domain.Entities;
using Octopus.Conductor.Infrastructure.RabbitMQ.Interfaces;
using Octopus.Conductor.Infrastructure.RabbitMQ.Service;
using Octopus.Conductor.Infrastructure.WorkerService.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.WorkerService.Services
{
    public class FolderListener : IFolderListener
    {
        private readonly IRepository _repository;
        private readonly IMessagePublisher _publisher;
        private ConcurrentQueue<Exception> _exceptions;

        public FolderListener(IRepository repository,
            IMessagePublisher publisher)
        {
            _repository = repository;
            _publisher = publisher;
        }

        public async Task MoveEntityFilesAsync(CancellationToken cancellationToken = default)
        {
            _exceptions = new ConcurrentQueue<Exception>();

            try
            {
                var descriptions = await _repository
                    .GetAllAsync<ConductorEntityDescription>(cancellationToken);

                ProcessDirectoriesInParallel(descriptions, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _exceptions.Enqueue(ex);
            }

            if (_exceptions.Count > 0)
                throw new AggregateException(_exceptions);
        }

        private void ProcessDirectoriesInParallel(
            IEnumerable<ConductorEntityDescription> descriptions,
            CancellationToken cancellationToken)
        {
            Parallel.ForEach(descriptions,
                new ParallelOptions { CancellationToken = cancellationToken }, desc =>
              {
                  var inputDirInfo = new DirectoryInfo(desc.InputDirectory);

                  if (!Directory.Exists(desc.OutputDirectory))
                      Directory.CreateDirectory(desc.OutputDirectory);

                  inputDirInfo.EnumerateFiles("*", SearchOption.AllDirectories)
                  .ToList()
                  .ForEach(info =>
                  {
                      cancellationToken.ThrowIfCancellationRequested();
                      MoveFile(info, desc);
                  });
              });
        }

        private void MoveFile(FileInfo fileInfo, ConductorEntityDescription desc)
        {
            try
            {
                var destFilePath = Path.Combine(desc.OutputDirectory, fileInfo.Name);
                File.Move(fileInfo.FullName, destFilePath);
                _publisher.Publish(
                new
                {
                    Type = desc.EntityType,
                    Path = destFilePath
                }, "demo-channel", "demo-exchange", "demo-key");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _exceptions.Enqueue(ex);
            }
        }
    }
}
