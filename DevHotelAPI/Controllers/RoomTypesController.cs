using AutoMapper;
using DevHotelAPI.Dtos;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/room-types")]
public class RoomTypesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IRoomTypeRepository _roomTypeRepository;
    private readonly IValidator<RoomType> _validator;


    public RoomTypesController( IMapper mapper, IRoomTypeRepository roomTypeRepository, IValidator<RoomType> validator)
    {
        _mapper = mapper;   
        _roomTypeRepository = roomTypeRepository;
        _validator = validator;
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoomType(int id)
    {
        await _roomTypeRepository.DeleteRoomTypeAsync(id);
        return NoContent();
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

        await _roomTypeRepository.AddRoomTypeAsync(roomType);
        return CreatedAtAction(nameof(GetRoomType), new { id = roomType.Id }, roomType);
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
        }
        catch (DbUpdateConcurrencyException)
        {
            var existingRoom = await _roomTypeRepository.GetRoomTypeByIdAsync(id);
            if (existingRoom == null)
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }
}
