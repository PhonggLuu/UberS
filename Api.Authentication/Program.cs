using Api.Authentication.Extensions;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Reflection;
using UberSystem.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
	.AddOData(opt => opt
		.EnableQueryFeatures()
		.AddRouteComponents("odata", GetEdmModel()));

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
	opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
	{
		Version = "v1",
		Title = "UberSystem.Api.Authentication",
		Description = "An ASP.NET Core Web API for managing customers",
		TermsOfService = new Uri("https://lms-hcmuni.fpt.edu.vn/course/view.php?id=2110"),
		Contact = new Microsoft.OpenApi.Models.OpenApiContact
		{
			Name = "Contact",
			Url = new Uri("https://lms-hcmuni.fpt.edu.vn/course/view.php?id=2110")
		},
		License = new Microsoft.OpenApi.Models.OpenApiLicense
		{
			Name = "License",
			Url = new Uri("https://lms-hcmuni.fpt.edu.vn/course/view.php?id=2110")
		}
	});

	// Set the comments path for the Swagger JSON and UI.
	var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Database connection and Dependency Injection
var connectionString = builder.Configuration.GetConnectionString("Default");
var configuration = builder.Configuration;
builder.Services.AddDatabase(configuration).AddServices(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseRouting();

app.UseHttpsRedirection();

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
	builder.EntitySet<User>("Users");
	return builder.GetEdmModel();
}
