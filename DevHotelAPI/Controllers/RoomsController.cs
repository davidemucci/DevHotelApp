using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using FluentValidation;
using AutoMapper;
using DevHotelAPI.Dtos;

namespace DevHotelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomRepository _repository;
        private readonly IValidator<Room> _validator;
        private readonly IMapper _mapper;

        public RoomsController(IMapper mapper, IRoomRepository repository, IValidator<Room> validator)
        {
            _mapper = mapper;
            _repository = repository;
            _validator = validator;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms()
        {
            var rooms = await _repository.GetAllRoomAsync();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDto>> GetRoom(int id)
        {
            var room = await _repository.GetByIdRoomAsync(id);

            if (room == null)
                return NotFound();

            return Ok(room);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, RoomDto roomDto)
        {
            if (id != roomDto.Number)
                return BadRequest();

            if(ModelState.IsValid)
                return BadRequest(ModelState);

            var room = _mapper.Map<Room>(roomDto);

            if (!_validator.Validate(room).IsValid)
                return BadRequest(_validator.Validate(room).Errors);

            try
            {
                await _repository.UpdateRoomAsync(room);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await RoomExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Room>> PostRoom(RoomDto roomDto)
        {
            if (ModelState.IsValid)
                return BadRequest(ModelState);

            var room = _mapper.Map<Room>(roomDto);

            if (!_validator.Validate(room).IsValid)
                return BadRequest(_validator.Validate(room).Errors);

            await _repository.AddRoomAsync(room);
            return CreatedAtAction("GetRoom", new { id = room.Number }, room);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _repository.GetByIdRoomAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            await _repository.DeleteRoomAsync(id);
            return NoContent();
        }

        private async Task<bool> RoomExists(int id)
        {
            var room = await _repository.GetByIdRoomAsync(id);
            return room != null;
        }
    }
}
