using AutoMapper;
using Azure;
using Azure.Core;
using DevHotelAPI.Dtos;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DevHotelAPI.Controllers
{
    [Route("api/reservations")]
    [Authorize(Roles = "Administrator,Consumer")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IReservationRepository _repository;
        private readonly IValidator<Reservation> _validator;
        public ReservationsController(IMapper mapper, IReservationRepository repository, IValidator<Reservation> validator)
        {
            _mapper = mapper;
            _repository = repository;
            _validator = validator;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(Guid id)
        {
            var userName = HttpContext?.User?.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
                return BadRequest("User not found");

            if (!await _repository.ReservationExistsAsync(id))
                return NotFound();

            try
            {
                await _repository.DeleteReservationAsync(id, userName);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDto>> GetReservation(Guid id)
        {
            var userName = HttpContext?.User?.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
                return BadRequest("User not found");

            var reservation = await _repository.GetReservationByIdAsync(id, userName);

            if (reservation == null)
                return NotFound();

            var reservationDto = _mapper.Map<ReservationDto>(reservation);
            return Ok(reservationDto);
        }

        [HttpGet]
        [Route("get-reservation-by-customer-id")]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservationByCustomerId(Guid customerId)
        {
            var userName = HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return BadRequest("User not found");

            var reservations = await _repository.GetReservationsByCustomerIdAsync(customerId, userName);

            return Ok(reservations != null ? _mapper.Map<List<ReservationDto>>(reservations) : null);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations()
        {
            var reservations = await _repository.GetAllReservationsAsync();
            var reservationDtos = _mapper.Map<IEnumerable<ReservationDto>>(reservations);
            return Ok(reservationDtos);
        }

        [HttpPost]
        public async Task<ActionResult<ReservationDto>> PostReservation(ReservationDto reservationDto)
        {
            var userName = HttpContext?.User?.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
                return BadRequest("User not found");

            var reservation = _mapper.Map<Reservation>(reservationDto);

            if (!_validator.Validate(reservation).IsValid)
                return BadRequest(_validator.Validate(reservation).Errors);

            if (!await _repository.CheckIfRoomIsAvailableAsync(reservation))
                return BadRequest("The selected room is not available for the specified dates. Please choose different dates or another room.");

            try
            {
                await _repository.AddReservationAsync(reservation, userName);
                return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservationDto);
            }
            catch (Exception ex)
            {
                return BadRequest("Can't add reservation.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(Guid id, ReservationDto reservationDto)
        {
            var userName = HttpContext?.User?.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
                return BadRequest("User not found");

            if (id != reservationDto.Id)
                return BadRequest();

            var reservation = _mapper.Map<Reservation>(reservationDto);

            if (!_validator.Validate(reservation).IsValid)
                return BadRequest(_validator.Validate(reservation).Errors);

            if (!await _repository.CheckIfRoomIsAvailableAsync(reservation))
                return BadRequest("The selected room is not available for the specified dates. Please choose different dates or another room.");

            try
            {
                await _repository.UpdateReservationAsync(reservation, userName);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repository.ReservationExistsAsync(id))
                    return NotFound();
                else
                    throw;
            }
            catch (Exception ex)
            {
                return BadRequest($"Can't modify the reservation with id {id}");
            }


            return NoContent();
        }
    }
}
