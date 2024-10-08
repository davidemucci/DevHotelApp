using DevHotelAPI.Contexts;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using DevHotelAPI.Services.Mapper;
using DevHotelAPI.Services.Repositories;
using DevHotelAPI.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DevHotelAPI.Contexts.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using DevHotelAPI.Services.Utility;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    builder.Services.AddControllers(opt => opt.RespectBrowserAcceptHeader = true)
        .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        );

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelDevAPI", Version = "v1" });
        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
        });
    });

    builder.Services.AddDbContext<HotelDevContext>(opt => opt.UseSqlServer(
        builder.Configuration.GetConnectionString("HotelDevConnectionString"))
    );

    builder.Services.AddDbContext<IdentityContext>(opt => opt.UseSqlServer(
        builder.Configuration.GetConnectionString("IdentityHotelDevConnectionString")).LogTo(Console.WriteLine, LogLevel.Information)
    );

    builder.Services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()
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
    });

    builder.Services.AddScoped<IBogusRepository, BogusRepository>();
    builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
    builder.Services.AddScoped<IRoomRepository, RoomRepository>();
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
    builder.Services.AddScoped<IAccountRepository, AccountRepository>();
    builder.Services.AddScoped<IHandleExceptionService, HandleExceptionService>();


    builder.Services.AddAutoMapper(typeof(MainMapperProfile));
    builder.Services.AddScoped<IValidator<RoomType>, RoomTypeValidator>();
    builder.Services.AddScoped<IValidator<Room>, RoomValidator>();
    builder.Services.AddScoped<IValidator<Customer>, CustomerValidator>();
    builder.Services.AddScoped<IValidator<Reservation>, ReservationValidator>();



    var app = builder.Build();
    // Configure the HTTP request pipeline.

    app.UseSerilogRequestLogging();
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        var logger = app.Services.GetRequiredService<Logger>();

        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/html";

                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionHandlerPathFeature?.Error != null)
                {
                    logger.Error(exceptionHandlerPathFeature.Error, "Unhandled exception");
                }

                await context.Response.WriteAsync("<html><body>\n");
                await context.Response.WriteAsync("An error occurred. Please try again later.<br>\n");
                await context.Response.WriteAsync("</body></html>\n");
            });
        });
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}catch(HostAbortedException)
{
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}