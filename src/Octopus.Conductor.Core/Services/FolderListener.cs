using Octopus.Conductor.Core.Entities;
using Octopus.Conductor.Core.Interfaces;
using System;
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

            if (descriptions == null)
                throw new ArgumentNullException(nameof(descriptions), "");

            Parallel.ForEach(descriptions,
                new ParallelOptions { CancellationToken = cancellationToken }, MoveFilesFromDirectory);
        }

        private void MoveFilesFromDirectory(ConductorEntityDescription description)
        {
            var files = Directory.GetFiles(description.InputDirectory, "*.*", SearchOption.AllDirectories);
            if (!Directory.Exists(description.OutputDirectory))
                Directory.CreateDirectory(description.OutputDirectory);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                File.Move(fileInfo.FullName, Path.Combine(description.OutputDirectory, fileInfo.Name));
            }
        }
    }
}
