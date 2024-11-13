using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Text.Json.Serialization;
using UberSystem.Api.Customer.Extensions;
using UberSystem.Domain.Entities;
using UberSystem.Service.Middleware;

var builder = WebApplication.CreateBuilder(args);


var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Customer>("Customers");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    })
    .AddOData(
    options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents(
        "odata",
        modelBuilder.GetEdmModel()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;
builder.Services.AddDatabase(configuration);
var app = builder.Build();
app.UseMiddleware<AddBearerMiddleware>();
app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(/*c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Uber System API V1");
		c.RoutePrefix = string.Empty; // Đặt Swagger UI ở gốc
	}*/);
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
