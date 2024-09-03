using DevHotelAPI.Entities;
using DevHotelAPI.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class RoomTypesController : ControllerBase
{
    private readonly IRoomTypeRepository _roomTypeRepository;

    public RoomTypesController(IRoomTypeRepository roomTypeRepository)
    {
        _roomTypeRepository = roomTypeRepository;
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

        return roomType;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutRoomType(int id, RoomType roomType)
    {
        if (id != roomType.Id)
        {
            return BadRequest();
        }

        try
        {
            await _roomTypeRepository.UpdateRoomTypeAsync(roomType);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await _roomTypeRepository.GetRoomTypeByIdAsync(id) == null)
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
    public async Task<ActionResult<RoomType>> PostRoomType(RoomType roomType)
    {
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
