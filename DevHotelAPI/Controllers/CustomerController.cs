using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevHotelAPI.Contexts;
using DevHotelAPI.Dtos;
using AutoMapper;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Serilog.Core;

namespace DevHotelAPI.Controllers
{
    [Route("api/customers")]
    [Authorize(Roles = "Consumer,Administrator")]
    [ApiController]
    public class CustomerController(Logger logger, IMapper mapper, ICustomerRepository repository, IValidator<Customer> validator) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICustomerRepository _repository = repository;
        private readonly IValidator<Customer> _validator = validator;
        private readonly Logger _logger = logger;

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
                return BadRequest($"User not found");

            if (!await _repository.CustomerExistsAsync(id))
                return NotFound();
            try
            {
                await _repository.DeleteCustomerAsync(id, userName);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }catch(ArgumentNullException ex)
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
        public async Task<ActionResult<CustomerDto>> GetCustomer(Guid id)
        {
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
                return BadRequest($"User not found");

            try
            {
                var customer = await _repository.GetCustomerByIdAsync(id, userName);
                if (customer == null)
                    return NotFound();

                var customerDto = _mapper.Map<CustomerDto>(customer);
                return Ok(customerDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest($"Error getting customer with id {id}");
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            var customers = await _repository.GetAllCustomersAsync();
            var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
            return Ok(customerDtos);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<CustomerDto>> PostCustomer(CustomerDto customerDto)
        {
            var customer = _mapper.Map<Customer>(customerDto);

            if (!_validator.Validate(customer).IsValid)
                return BadRequest(_validator.Validate(customer).Errors);

            try
            {
                await _repository.AddCustomerAsync(customer);
                return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customerDto);
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest("Error during saving customer in the database");
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest("Error during add customer to the database");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(Guid id, CustomerDto customerDto)
        {
            if (id != customerDto.Id)
                return BadRequest();

            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
                return BadRequest($"User not found");

            var customer = _mapper.Map<Customer>(customerDto);

            if (!_validator.Validate(customer).IsValid)
                return BadRequest(_validator.Validate(customer).Errors);

            try
            {
                await _repository.UpdateCustomerAsync(customer, userName);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await _repository.CustomerExistsAsync(id))
                    return NotFound();
                else
                {
                    _logger.Error(ex.Message, ex);
                    return BadRequest($"Error modifing customer with id {id}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return BadRequest($"Error modifing customer with id {id}");
            }
        }
    }
}
