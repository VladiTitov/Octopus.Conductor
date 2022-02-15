using AutoMapper;
using Octopus.Conductor.Domain.Entities;
using Octopus.Conductor.WebApi.Models;

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
