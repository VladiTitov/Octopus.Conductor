using AutoMapper;
using Octopus.Conductor.Core.Entities;
using Octopus.Conductor.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Conductor.WebApi.Profiles
{
    public class EntityDescriptionProfile : Profile
    {
        public EntityDescriptionProfile()
        {
            CreateMap<CreateEntityDescriptionDto, ConductorEntityDescription>();
            CreateMap<ConductorEntityDescription, ReadEntityDescriptionDto>();
        }
    }
}
