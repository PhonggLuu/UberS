using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using UberSystem.Api.Driver.Extensions;
using UberSystem.Domain.Entities;
using UberSystem.Service.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
})
.AddOData(opt => opt
		.EnableQueryFeatures()
		.AddRouteComponents("odata", GetEdmModel()));

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
		c.RoutePrefix = string.Empty; // ??t Swagger UI ? g?c
	}*/);
}

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllers();
});

app.Run();

static IEdmModel GetEdmModel()
{
	var builder = new ODataConventionModelBuilder();
	// Register your entity sets here
	builder.EntitySet<Driver>("Drivers");
	return builder.GetEdmModel();
}