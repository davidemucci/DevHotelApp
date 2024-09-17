using AutoMapper;
using DevHotelAPI.Dtos;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace DevHotelAPI.Controllers
{
    [ApiController]
    [Route("api/room-types")]
    public class RoomTypesController(IMapper mapper, IRoomTypeRepository roomTypeRepository, IValidator<RoomType> validator, ILogger logger) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;
        private readonly IRoomTypeRepository _roomTypeRepository = roomTypeRepository;
        private readonly IValidator<RoomType> _validator = validator;
        private readonly ILogger _logger = logger;

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomType(int id)
        {
            try
            {
                await _roomTypeRepository.DeleteRoomTypeAsync(id);
                return NoContent();
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex.Message, ex);
                return BadRequest($"Error deleting customer with id {id}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomType>> GetRoomType(int id)
        {
            var roomType = await _roomTypeRepository.GetRoomTypeByIdAsync(id);
            if (roomType == null)
                return NotFound();

            return Ok(roomType);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomType>>> GetRoomTypes()
        {
            var roomTypes = await _roomTypeRepository.GetAllRoomTypesAsync();
            return Ok(roomTypes);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult<RoomType>> PostRoomType(RoomTypeDto roomTypeDto)
        {
            var roomType = _mapper.Map<RoomType>(roomTypeDto);

            if (!_validator.Validate(roomType).IsValid)
                return BadRequest(_validator.Validate(roomType).Errors);
            try
            {
                await _roomTypeRepository.AddRoomTypeAsync(roomType);

                return CreatedAtAction(nameof(GetRoomType), new { id = roomType.Id }, roomType);
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest("Error during saving roomType in the database");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest("Error during add roomType to the database");
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomType(int id, RoomTypeDto roomTypeDto)
        {
            if (id != roomTypeDto.Id || id == 0 || roomTypeDto.Id == 0)
                return BadRequest(ModelState);

            var roomType = _mapper.Map<RoomType>(roomTypeDto);

            if (!_validator.Validate(roomType).IsValid)
                return BadRequest(_validator.Validate(roomType).Errors);

            try
            {
                await _roomTypeRepository.UpdateRoomTypeAsync(roomType);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var existingRoom = await _roomTypeRepository.GetRoomTypeByIdAsync(id);
                if (existingRoom == null)
                    return NotFound();
                else
                {
                    _logger.Error(ex.Message, ex);
                    return BadRequest($"Database error updating roomType with id {id}");
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest($"Database error updating roomType with id {id}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest($"App Error updating roomType with id {id}");
            }
        }
    }
}