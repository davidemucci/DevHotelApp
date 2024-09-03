using DevHotelAPI.Contexts;
using DevHotelAPI.Services;
using DevHotelAPI.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(opttions =>
{
    opttions.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HotelDevContext>(opt => opt.UseSqlServer(
    builder.Configuration.GetConnectionString("HotelDevConnectionString")));
builder.Services.AddScoped<IBogusRepository, BogusRepository>();
builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
