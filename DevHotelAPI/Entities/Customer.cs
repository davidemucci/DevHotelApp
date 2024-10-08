﻿using DevHotelAPI.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace DevHotelAPI.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public Guid IdentityUserId { get; set; }
        public required string Email { get; set; }
        public string? Address { get; set; }
        public string? Name { get; set; }
        public List<Reservation>? Reservations { get; set; }
        public string? Surname { get; set; }
    }
}
