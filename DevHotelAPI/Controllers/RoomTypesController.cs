using AutoMapper;
using DevHotelAPI.Dtos;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class RoomTypesController : ControllerBase
{
    private readonly IRoomTypeRepository _roomTypeRepository;
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IValidator<RoomType> _validator;


    public RoomTypesController(DbContext dbContext, IMapper mapper, IRoomTypeRepository roomTypeRepository, IValidator<RoomType> validator)
    {
        _dbContext = dbContext;
        _mapper = mapper;   
        _roomTypeRepository = roomTypeRepository;
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomType>>> GetRoomTypes()
    {
        var roomTypes = await _roomTypeRepository.GetAllRoomTypesAsync();
        return Ok(roomTypes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomType>> GetRoomType(int id)
    {
        var roomType = await _roomTypeRepository.GetRoomTypeByIdAsync(id);
        if (roomType == null)
        {
            return NotFound();
        }

        return Ok(roomType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutRoomType(int id, RoomType roomType)
    {
        if (id != roomType.Id || id == 0 || roomType.Id == 0)
        {
            return BadRequest();
        }

        try
        {
            await _roomTypeRepository.UpdateRoomTypeAsync(roomType);
        }
        catch (DbUpdateConcurrencyException)
        {
            var existingRoom = await _roomTypeRepository.GetRoomTypeByIdAsync(id);
            if (existingRoom == null)
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<RoomType>> PostRoomType(RoomTypeDto roomTypeDto)
    {

        if(!ModelState.IsValid) 
            return BadRequest(ModelState);
        
        var roomType = _mapper.Map<RoomType>(roomTypeDto);

        if (!_validator.Validate(roomType).IsValid)
            return BadRequest(_validator.Validate(roomType).Errors);
        

        await _roomTypeRepository.AddRoomTypeAsync(roomType);
        return CreatedAtAction(nameof(GetRoomType), new { id = roomType.Id }, roomType);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoomType(int id)
    {
        await _roomTypeRepository.DeleteRoomTypeAsync(id);
        return NoContent();
    }
}
