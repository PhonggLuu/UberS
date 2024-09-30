using Microsoft.EntityFrameworkCore;
using UberSystem.Domain.Interfaces;
using UberSystem.Domain.Interfaces.Services;
using UberSystem.Infrastructure;
using UberSystem.Service;
using UberSytem.Dto;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Default");
var configuration = builder.Configuration;
builder.Services.AddDbContext<UberSystemDbContext>(options =>
	options.UseSqlServer(connectionString)); // Adjust as needed
builder.Services.AddScoped<Func<UberSystemDbContext>>(provider => () => provider.GetService<UberSystemDbContext>());
builder.Services.AddScoped<DbFactory>(); // Ensure the correct lifetime
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<ILocateService, LocateService>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddAutoMapper(typeof(MappingProfileExtension));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
