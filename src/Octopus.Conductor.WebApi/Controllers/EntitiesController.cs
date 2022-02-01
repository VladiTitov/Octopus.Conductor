using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Octopus.Conductor.Core.Entities;
using Octopus.Conductor.Core.Interfaces;
using Octopus.Conductor.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Conductor.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EntitiesController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public EntitiesController(
            IRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadEntityDescriptionDto>>> GetAllEntities()
        {
            var entities = await _repository.GetAllAsync<EntityDescription>();
            return Ok(_mapper.Map<IEnumerable<ReadEntityDescriptionDto>>(entities));
        }

        [HttpPost]
        public async Task<ActionResult<CreateEntityDescriptionDto>> CreateEntity([FromBody] CreateEntityDescriptionDto entityDescriptionDto)
        {
            var entity = _mapper.Map<EntityDescription>(entityDescriptionDto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            var readEntityDto = _mapper.Map<ReadEntityDescriptionDto>(entity);
            return CreatedAtRoute(nameof(GetEntityById), new { Id = readEntityDto.Id }, readEntityDto);
        }

        [HttpGet("{id}", Name = "GetEntityById")]
        public async Task<ActionResult<ReadEntityDescriptionDto>> GetEntityById(int id)
        {
            var entity = await _repository.GetByIdAsync<EntityDescription>(id);
            return Ok(_mapper.Map<ReadEntityDescriptionDto>(entity));
        }
    }
}
