using Octopus.Conductor.Core.Entities;
using Octopus.Conductor.Core.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Conductor.Core.Services
{
    public class FolderListener : IFolderListener
    {
        private readonly IRepository _repository;

        public FolderListener(IRepository repository)
        {
            _repository = repository;
        }

        public async Task MoveEntityFilesAsync(CancellationToken cancellationToken = default)
        {
            var descriptions = await _repository
                .GetAllAsync<ConductorEntityDescription>(cancellationToken);

            ProcessDirectoriesInParallel(descriptions, cancellationToken);
        }

        private void ProcessDirectoriesInParallel(
            IEnumerable<ConductorEntityDescription> descriptions,
            CancellationToken cancellationToken)
        {
            var exceptions = new ConcurrentQueue<Exception>();

            Parallel.ForEach(descriptions,
                new ParallelOptions { CancellationToken = cancellationToken }, desc =>
              {
                  try
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
                              exceptions.Enqueue(ex);
                          }
                      }
                  }
                  catch (Exception ex)
                  {
                      exceptions.Enqueue(ex);
                  }
              });

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }
    }
}
