using AutoMapper;
using DevHotelAPI.Dtos;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHotelAPI.Controllers
{
    [Route("api/reservations")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationRepository _repository;
        private readonly IValidator<Reservation> _validator;
        private readonly IMapper _mapper;

        public ReservationsController(IMapper mapper, IReservationRepository repository, IValidator<Reservation> validator)
        {
            _mapper = mapper;
            _repository = repository;
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations()
        {
            var reservations = await _repository.GetAllReservationsAsync();
            var reservationDtos = _mapper.Map<IEnumerable<ReservationDto>>(reservations);
            return Ok(reservationDtos);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservationByClientId(Guid clientId)
        {
            var reservations = await _repository.GetReservationsByClientIdAsync(clientId);

            return Ok(reservations != null ? _mapper.Map<List<ReservationDto>>(reservations) : null);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDto>> GetReservation(Guid id)
        {
            var reservation = await _repository.GetReservationByIdAsync(id);

            if (reservation == null)
                return NotFound();

            var reservationDto = _mapper.Map<ReservationDto>(reservation);
            return Ok(reservationDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(Guid id, ReservationDto reservationDto)
        {
            if (id != reservationDto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reservation = _mapper.Map<Reservation>(reservationDto);

            if (!_validator.Validate(reservation).IsValid)
                return BadRequest(_validator.Validate(reservation).Errors);

            if (!await _repository.CheckIfRoomIsAvailableAsync(reservation))
                return BadRequest("The selected room is not available for the specified dates. Please choose different dates or another room.");

            try
            {
                await _repository.UpdateReservationAsync(reservation);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repository.ReservationExistsAsync(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<ReservationDto>> PostReservation(ReservationDto reservationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reservation = _mapper.Map<Reservation>(reservationDto);

            if (!_validator.Validate(reservation).IsValid)
                return BadRequest(_validator.Validate(reservation).Errors);

            if (!await _repository.CheckIfRoomIsAvailableAsync(reservation))
                return BadRequest("The selected room is not available for the specified dates. Please choose different dates or another room.");


            await _repository.AddReservationAsync(reservation);
            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservationDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(Guid id)
        {
            var reservation = await _repository.GetReservationByIdAsync(id);
            if (reservation == null)
                return NotFound();

            await _repository.DeleteReservationAsync(id);
            return NoContent();
        }
    }
}
