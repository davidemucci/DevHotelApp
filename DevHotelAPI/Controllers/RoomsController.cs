﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using FluentValidation;
using AutoMapper;
using DevHotelAPI.Dtos;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace DevHotelAPI.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRoomRepository _repository;
        private readonly IValidator<Room> _validator;
        public RoomsController(IMapper mapper, IRoomRepository repository, IValidator<Room> validator)
        {
            _mapper = mapper;
            _repository = repository;
            _validator = validator;

        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _repository.GetByIdRoomAsync(id);
            if (room == null)
                return NotFound();

            await _repository.DeleteRoomAsync(id);
            return NoContent();
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
            return Ok(_mapper.Map<List<RoomDto>>(rooms));
        }

        [HttpGet]
        [Route("get-available-rooms")]
        public async Task<ActionResult<IEnumerable<IGrouping<int, Room>>>> GetRoomsAvailable(DateTime from, DateTime to, int people)
        {
            var rooms = await _repository.GetAllRoomsAvailableAsync(from, to, people);
            var roomsDto = _mapper.Map<List<RoomDto>>(rooms);

            return Ok(roomsDto.GroupBy(x => x.RoomTypeId).OrderBy(x => x.Key).ToList());
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult<RoomDto>> PostRoom(RoomDto roomDto)
        {
            var room = _mapper.Map<Room>(roomDto);

            if (!_validator.Validate(room).IsValid)
                return BadRequest(_validator.Validate(room).Errors);

            await _repository.AddRoomAsync(room);
            return CreatedAtAction("GetRoom", new RoomDto { Number = room.Number }, _mapper.Map<RoomDto>(room));
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

        private async Task<bool> RoomExists(int id)
        {
            var room = await _repository.GetByIdRoomAsync(id);
            return room != null;
        }
    }
}
