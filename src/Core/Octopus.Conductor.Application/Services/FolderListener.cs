using Octopus.Conductor.Application.Interfaces;
using Octopus.Conductor.Domain.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Conductor.Application.Services
{
    public class FolderListener : IFolderListener
    {
        private readonly IRepository _repository;
        private ConcurrentQueue<Exception> _exceptions;

        public FolderListener(IRepository repository)
        {
            _repository = repository;
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
            var exceptions = new ConcurrentQueue<Exception>();

            Parallel.ForEach(descriptions,
                new ParallelOptions { CancellationToken = cancellationToken }, desc =>
              {
                  var files = Directory.GetFiles(desc.InputDirectory, "*.*", SearchOption.AllDirectories);

                  if (!Directory.Exists(desc.OutputDirectory))
                      Directory.CreateDirectory(desc.OutputDirectory);

                  foreach (var file in files)
                  {
                      try
                      {
                          cancellationToken.ThrowIfCancellationRequested();
                          var fileInfo = new FileInfo(file);
                          File.Move(fileInfo.FullName, Path.Combine(desc.OutputDirectory, fileInfo.Name));
                      }
                      catch (Exception ex)
                      {
                          _exceptions.Enqueue(ex);
                      }
                  }
              });
        }
    }
}
