using Octopus.Conductor.Core.Entities;
using Octopus.Conductor.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Octopus.Conductor.Core.Services
{
    public class FolderListener : IFolderListener
    {
        public async Task MoveEntityFilesAsync(IEnumerable<ConductorEntityDescription> descriptions)
        {
            var tasksList = new List<Task>();

            foreach (var description in descriptions)
            {
                tasksList.Add(MoveFilesFromDirectory(description));
            }

            await Task.WhenAll(tasksList);
        }

        private Task MoveFilesFromDirectory(ConductorEntityDescription description)
        {
            return Task.Run(() =>
            {
                var files = Directory.GetFiles(description.InputDirectory, "*.*", SearchOption.AllDirectories);
                if (!Directory.Exists(description.OutputDirectory))
                    Directory.CreateDirectory(description.OutputDirectory);

                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    File.Move(fileInfo.FullName, Path.Combine(description.OutputDirectory, fileInfo.Name));
                }
            });
        }
    }
}
