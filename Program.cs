using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using robot_controller_api.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Register ADO implementations for dependency injection
builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandADO>();
builder.Services.AddScoped<IMapDataAccess, MapADO>();
builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandRepository>();
builder.Services.AddScoped<IMapDataAccess, MapRepository>();

// Add controllers
builder.Services.AddControllers();

// Swagger for API docs
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map attribute-routed controllers
app.MapControllers();

app.Run();
