using System;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using robot_controller_api.Authentication;
using robot_controller_api.Persistence;
using robot_controller_api.Services; // ← new

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------------------------
// Register repository‐pattern implementations for dependency injection
// ----------------------------------------------------------------------------
builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandRepository>();
builder.Services.AddScoped<IMapDataAccess, MapRepository>();
builder.Services.AddScoped<IUserDataAccess, UserRepository>();

// ----------------------------------------------------------------------------
// Add MVC controllers
// ----------------------------------------------------------------------------
builder.Services.AddControllers();

// ----------------------------------------------------------------------------
// Register new password‐hasher service
// ----------------------------------------------------------------------------
builder.Services.AddScoped<IPasswordHasherService, Argon2PasswordHasherService>();

// ----------------------------------------------------------------------------
// Configure Basic Authentication & Authorization policies
// ----------------------------------------------------------------------------
builder.Services
    .AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
        "BasicAuthentication", _ => { }
    );

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin"));

    options.AddPolicy("UserOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin", "User"));

    // ----------- Custom claim/policy for FirstName starts with A -------------
    options.AddPolicy("NameStartsWithAOnly", policy =>
        policy.RequireClaim("NameStartsWithA", "true"));
    // --------------------------------------------------------------------------------------
});

// ----------------------------------------------------------------------------
// Add Swagger/OpenAPI support (with XML comments & BasicAuth in UI)
// ----------------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Robot Controller API",
        Version = "v1",
        Description = "Backend service for Moon robot simulator.",
        Contact = new OpenApiContact
        {
            Name = "Lakmal Wijewardene",
            Email = "lakishwijewardene@gmail.com"
        }
    });

    // XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    // BasicAuth scheme for Swagger
    options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        Description = "Enter your username and password"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "basic"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ----------------------------------------------------------------------------
// Static files and Swagger UI
// ----------------------------------------------------------------------------
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.InjectStylesheet("/styles/theme-material.css");
        c.InjectStylesheet("/styles/robot.css");
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "robot-controller-api v1");
    });
}

// ----------------------------------------------------------------------------
// Middleware pipeline
// ----------------------------------------------------------------------------
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
