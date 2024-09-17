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
using Serilog.Core;
using System.Security.Claims;

namespace DevHotelAPI.Controllers
{
    [Route("api/reservations")]
    [Authorize(Roles = "Administrator,Consumer")]
    [ApiController]
    public class ReservationsController(IMapper mapper, IReservationRepository repository, IValidator<Reservation> validator, Logger logger) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;
        private readonly IReservationRepository _repository = repository;
        private readonly IValidator<Reservation> _validator = validator;
        private readonly Logger _logger = logger;

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(Guid id)
        {
            var userName = HttpContext?.User?.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
                return BadRequest("User not found");

            try
            {
                await _repository.DeleteReservationAsync(id, userName);
                return NoContent();
            }
            catch(UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(DbUpdateException ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest($"Database error deleting reservation with id {id}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest($"Error deleting reservation with id {id}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDto>> GetReservation(Guid id)
        {
            var userName = HttpContext?.User?.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
                return BadRequest("User not found");
            try
            {

                var reservation = await _repository.GetReservationByIdAsync(id, userName);
                if (reservation == null)
                    return NotFound();

                var reservationDto = _mapper.Map<ReservationDto>(reservation);
                return Ok(reservationDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest($"Error getting reservation with id {id}");
            }  
        }

        [HttpGet]
        [Route("get-reservation-by-customer-id")]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservationByCustomerId(Guid customerId)
        {
            var userName = HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return BadRequest("User not found");
            try
            {
                var reservations = await _repository.GetReservationsByCustomerIdAsync(customerId, userName);
                if(reservations is null)
                    return NotFound();

                return Ok(_mapper.Map<List<ReservationDto>>(reservations));
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest($"Error getrting reservation for customer with id {customerId}");
            }

        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations()
        {
            var reservations = await _repository.GetAllReservationsAsync();
            if(reservations is null)
                return NotFound();

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
            catch(UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);  
            }
            catch(DbUpdateException ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest($"Database error saving reservation for user {userName}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest($"Error adding reservation for user {userName}.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(Guid id, ReservationDto reservationDto)
        {
            var userName = HttpContext?.User?.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
                return BadRequest("User not found");

            if (id != reservationDto.Id)
                return BadRequest("Id param and reservation Id are not the same");

            var reservation = _mapper.Map<Reservation>(reservationDto);

            if (!_validator.Validate(reservation).IsValid)
                return BadRequest(_validator.Validate(reservation).Errors);

            if (!await _repository.CheckIfRoomIsAvailableAsync(reservation))
                return BadRequest("The selected room is not available for the specified dates. Please choose different dates or another room.");

            try
            {
                await _repository.UpdateReservationAsync(reservation, userName);
                return NoContent();
            }
            catch(UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await _repository.ReservationExistsAsync(id))
                    return NotFound();
                else
                {
                    _logger.Error(ex.Message, ex);
                    return BadRequest($"Error modifing customer with id {id}");
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex.Message, ex);
                return BadRequest($"Can't modify the reservation with id {id}");
            }
        }
    }
}
