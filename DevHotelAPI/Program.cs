using AutoMapper;
using DevHotelAPI.Contexts;
using DevHotelAPI.Dtos;
using DevHotelAPI.Entities;
using DevHotelAPI.Services;
using DevHotelAPI.Services.Contracts;
using DevHotelAPI.Services.Mapper;
using DevHotelAPI.Services.Repositories;
using DevHotelAPI.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using DevHotelAPI.Entities.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(opt => opt.RespectBrowserAcceptHeader = true)
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HotelDevContext>(opt => opt.UseSqlServer(
    builder.Configuration.GetConnectionString("HotelDevConnectionString"))
);

/*builder.Services.AddDbContext<IdentityContext>();*/
builder.Services.AddDbContext<IdentityContext>(opt => opt.UseSqlServer(
    builder.Configuration.GetConnectionString("IdentityHotelDevConnectionString"))

);


builder.Services.AddIdentity<UserProfile, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    var secret = builder.Configuration["JwtConfig:Secret"];
    var issuer = builder.Configuration["JwtConfig:ValidIssuer"];
    var audience = builder.Configuration["JwtConfig:ValidAudiences"];

    if (secret is null || issuer is null || audience is null)
        throw new ApplicationException("JWT is not set in the configuration");

    opt.SaveToken = true;
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidIssuer = issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };
} );

builder.Services.AddScoped<IBogusRepository, BogusRepository>();
builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();


builder.Services.AddAutoMapper(typeof(MainMapperProfile));
builder.Services.AddScoped<IValidator<RoomType>, RoomTypeValidator>();
builder.Services.AddScoped<IValidator<Room>, RoomValidator>();
builder.Services.AddScoped<IValidator<Customer>, CustomerValidator>();
builder.Services.AddScoped<IValidator<Reservation>, ReservationValidator>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    /*    var context = app.Services.GetRequiredService<HotelDevContext>();
        context.Database.EnsureCreated();*/
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
