using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Conductor.WebApi.Models
{
    public class CreateEntityDescriptionDto
    {
        [Required]
        public string EntityType { get; set; }
        [Required]
        public string InputDirectory { get; set; }
        [Required]
        public string OutputDirectory { get; set; }
    }
}
