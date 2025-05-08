using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using robot_controller_api.Persistence;
using System;
using System.IO;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------------------------
// Register data-access implementations for dependency injection
// ----------------------------------------------------------------------------
// ADO.NET implementations
builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandADO>();
builder.Services.AddScoped<IMapDataAccess, MapADO>();
// FastMember-ORM implementations
builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandRepository>();
builder.Services.AddScoped<IMapDataAccess, MapRepository>();

// ----------------------------------------------------------------------------
// Add MVC controllers
// ----------------------------------------------------------------------------
builder.Services.AddControllers();

// ----------------------------------------------------------------------------
// Add Swagger/OpenAPI support
//    - Generates swagger.json file
//    - Pulls in XML comments for auto-doc
// ----------------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Define API
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Robot Controller API",
        Version = "v1",
        Description = "New backend service that provides resources for the Moon robot simulator.",
        Contact = new OpenApiContact
        {
            Name = "Lakmal Wijewardene",
            Email = "lakishwijewardene@gmail.com"
        }
    });

    // Include XML comments file if it exists
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// ----------------------------------------------------------------------------
// Serve static files (wwwroot) so CSS, images & favicons are exposed
// ----------------------------------------------------------------------------
app.UseStaticFiles();

// ----------------------------------------------------------------------------
// Enable Swagger UI in Development only, with custom theming
// ----------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    // Serve the generated OpenAPI JSON at "/swagger/v1/swagger.json"
    app.UseSwagger();

    // Serve the interactive UI at "/swagger"
    app.UseSwaggerUI(c =>
    {
        // Material theme (base look)
        c.InjectStylesheet("/styles/theme-material.css");

        // Your overrides (logo swap + colour tweaks)
        c.InjectStylesheet("/styles/robot.css");

        // Point to swagger doc
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "robot-controller-api v1");
    });
}

// ----------------------------------------------------------------------------
// Other middleware
// ----------------------------------------------------------------------------
app.UseHttpsRedirection();

// ----------------------------------------------------------------------------
// Map attribute-routed controllers (API endpoints)
// ----------------------------------------------------------------------------
app.MapControllers();

app.Run();
