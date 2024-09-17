using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using FluentValidation;
using AutoMapper;
using DevHotelAPI.Dtos;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Serilog.Core;
using DevHotelAPI.Services.Repositories;

namespace DevHotelAPI.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomsController(IMapper mapper, IRoomRepository repository, IValidator<Room> validator, Logger logger) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;
        private readonly IRoomRepository _repository = repository;
        private readonly IValidator<Room> _validator = validator;
        private readonly Logger _logger = logger;

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _repository.GetByIdRoomAsync(id);
            if (room == null)
                return NotFound();

            try
            {
                await _repository.DeleteRoomAsync(room);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger?.Error(ex.Message, ex);
                return BadRequest($"Error deleting customer with id {id}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDto>> GetRoom(int id)
        {
            var room = await _repository.GetByIdRoomAsync(id);

            if (room == null)
                return NotFound();

            return Ok(_mapper.Map<RoomDto>(room));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms()
        {
            var rooms = await _repository.GetAllRoomAsync();
            if(rooms == null)
                return NotFound();

            return Ok(_mapper.Map<List<RoomDto>>(rooms));
        }

        [HttpGet]
        [Route("get-available-rooms")]
        public async Task<ActionResult<IEnumerable<IGrouping<int, Room>>>> GetRoomsAvailable(DateTime from, DateTime to, int people)
        {
            var rooms = await _repository.GetAllRoomsAvailableAsync(from, to, people);

            if(rooms == null)
                return NotFound();

            var roomsDto = _mapper.Map<List<RoomDto>>(rooms);

            return Ok(roomsDto.GroupBy(x => x.RoomTypeId).OrderBy(x => x.Key).ToList());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<RoomDto>> PostRoom(RoomDto roomDto)
        {
            var room = _mapper.Map<Room>(roomDto);

            if (!_validator.Validate(room).IsValid)
                return BadRequest(_validator.Validate(room).Errors);
            try
            {
                await _repository.AddRoomAsync(room);
                return CreatedAtAction("GetRoom", new RoomDto { Number = room.Number }, _mapper.Map<RoomDto>(room));
            }
            catch(DbUpdateException) 
            {
                return BadRequest("Database error saving new room");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest("Error during app new room");
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, RoomDto roomDto)
        {
            if (id != roomDto.Number)
                return BadRequest();

            var room = _mapper.Map<Room>(roomDto);

            if (!_validator.Validate(room).IsValid)
                return BadRequest(_validator.Validate(room).Errors);

            try
            {
                await _repository.UpdateRoomAsync(room);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var existingRoom = await _repository.GetByIdRoomAsync(id);
                if (existingRoom == null)
                    return NotFound();
                else
                {
                    _logger.Error(ex.Message, ex);
                    return BadRequest($"Database error updating room with id {id}");
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest($"Database error updating room with id {id}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest($"App Error updating room with id {id}");
            }
        }

        private async Task<bool> RoomExists(int id)
        {
            var room = await _repository.GetByIdRoomAsync(id);
            return room != null;
        }
    }
}
