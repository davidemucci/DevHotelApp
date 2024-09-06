using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevHotelAPI.Contexts;
using DevHotelAPI.Dtos;
using AutoMapper;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using FluentValidation;

namespace DevHotelAPI.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IClientRepository _repository;
        private readonly IValidator<Client> _validator;
        public ClientsController(IMapper mapper, IClientRepository repository, IValidator<Client> validator)
        {
            _mapper = mapper;
            _repository = repository;
            _validator = validator;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            var client = await _repository.GetClientByIdAsync(id);
            if (client == null)
                return NotFound();

            await _repository.DeleteClientAsync(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> GetClient(Guid id)
        {
            var client = await _repository.GetClientByIdAsync(id);

            if (client == null)
                return NotFound();

            var clientDto = _mapper.Map<ClientDto>(client);
            return Ok(clientDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            var clients = await _repository.GetAllClientsAsync();
            var clientDtos = _mapper.Map<IEnumerable<ClientDto>>(clients);
            return Ok(clientDtos);
        }
        [HttpPost]
        public async Task<ActionResult<ClientDto>> PostClient(ClientDto clientDto)
        {
            var client = _mapper.Map<Client>(clientDto);

            if (!_validator.Validate(client).IsValid)
                return BadRequest(_validator.Validate(client).Errors);

            await _repository.AddClientAsync(client);
            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, clientDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(Guid id, ClientDto clientDto)
        {
            if (id != clientDto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = _mapper.Map<Client>(clientDto);

            if (!_validator.Validate(client).IsValid)
                return BadRequest(_validator.Validate(client).Errors);

            try
            {
                await _repository.UpdateClientAsync(client);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repository.ClientExistsAsync(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }
    }
}
