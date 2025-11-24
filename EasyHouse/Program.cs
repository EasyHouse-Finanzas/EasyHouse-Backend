using System;
using System.Text;
using EasyHouse.IAM.Application.SecurityCommandServices;
using EasyHouse.IAM.Domain.Repositories;
using EasyHouse.IAM.Domain.Services;
using EasyHouse.IAM.Infrastructure;
using EasyHouse.Shared.Domain.Repositories;
using EasyHouse.Shared.Infrastructure;
using EasyHouse.Simulations.Application;
using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Models.Repository;
using EasyHouse.Simulations.Domain.Models.Validators;
using EasyHouse.Simulations.Domain.Services;
using EasyHouse.Simulations.Infrastructure.Persistence.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// ---------------------------
// DATABASE / CONTEXT
// ---------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// ---------------------------
// REPOSITORIES / SERVICES
// ---------------------------
builder.Services.AddScoped<ISimulationRepository, SimulationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ISimulationCommandService, SimulationCommandService>();
builder.Services.AddScoped<IValidator<CreateSimulationCommand>, CreateSimulationCommandValidator>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IClientCommandService, ClientCommandService>();
builder.Services.AddScoped<IValidator<CreateClientCommand>, CreateClientCommandValidator>();

builder.Services.AddScoped<ISimulationCalculatorService, SimulationCalculatorService>();

builder.Services.AddScoped<IHouseRepository, HouseRepository>();
builder.Services.AddScoped<IHouseCommandService, HouseCommandService>();
builder.Services.AddScoped<IValidator<CreateHouseCommand>, CreateHouseCommandValidator>();

builder.Services.AddScoped<IConfigRepository, ConfigRepository>();
builder.Services.AddScoped<IConfigCommandService, ConfigCommandService>();
//builder.Services.AddScoped<IValidator<CreateConfigCommand>, CreateConfigCommandValidator>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// ---------------------------
// CONTROLLERS / SWAGGER
// ---------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger + JWT config
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EasyHouse API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese: Bearer {token}"
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

// ---------------------------
// AUTHENTICATION / JWT
// ---------------------------
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey) || string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new Exception("Jwt config missing in appsettings.json (Jwt:Key, Jwt:Issuer, Jwt:Audience).");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// ---------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyHouse API v1"));
}

app.UseHttpsRedirection();

// AUTH middleware must be before MapControllers
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
