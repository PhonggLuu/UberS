using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UberSystem.Domain.Interfaces;
using UberSystem.Domain.Interfaces.Services;
using UberSystem.Infrastructure;
using UberSystem.Service;
using UberSytem.Domain;

namespace UberSystem.Api.Customer.Extensions;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Add needed instances for database
	/// </summary>
	/// <param name="services"></param>
	/// <param name="configuration"></param>
	/// <returns></returns>
	public static IServiceCollection AddDatabase(this IServiceCollection services, ConfigurationManager configuration)
	{
		// Configure DbContext with Scoped lifetime  
		services.AddDbContext<UberSystemDbContext>(options =>
		{
			options.UseSqlServer(configuration.GetConnectionString("Default"),
				sqlOptions => sqlOptions.CommandTimeout(120));
			// options.UseLazyLoadingProxies();
		});
		services.AddHttpClient(); // Register IHttpClientFactory
		services.AddScoped<Func<UberSystemDbContext>>(provider => () => provider.GetService<UberSystemDbContext>());
		services.AddScoped<DbFactory>(); // Ensure the correct lifetime
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddScoped<IDriverService, DriverService>();
		services.AddScoped<IRatingService, RatingService>();
		services.AddScoped<ILocateService, LocateService>();
		services.AddScoped<INotificationService, NotificationService>();
		services.AddScoped<ITripService, TripService>();
		services.AddAutoMapper(typeof(MappingProfileExtension));
		services.AddSignalR();
		//        services.AddIdentity<User, IdentityRole>()
		//.AddEntityFrameworkStores<UberSystemDbContext>()
		//.AddDefaultTokenProviders();
		// Configure JWT authentication
		var jwtSettings = configuration.GetSection("JwtSettings");
		var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidIssuer = jwtSettings["Issuer"],
					ValidAudience = jwtSettings["Audience"],
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true
				};
			});

		services.AddAuthorization();

		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "Uber System API", Version = "v1" });
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				In = ParameterLocation.Header,
				Description = "Please enter your bearer token",
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey,
			});
			c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
						new string[] {}
					}
				});
		});

		// AutoMapper
		services.AddAutoMapper(typeof(MappingProfileExtension));
		return services;
	}
}
