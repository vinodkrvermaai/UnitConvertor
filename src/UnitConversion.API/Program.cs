using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using UnitConversion.API.Middleware;
using UnitConversion.Application;
using UnitConversion.Infrastructure;

// Bootstrap console logging until Serilog reads appsettings (including daily file sink).
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, _, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext());

    // Composition root: register layers, then HTTP pipeline (validation → controllers → middleware).
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHealthChecks();

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed"
            };
            return new BadRequestObjectResult(problemDetails);
        };
    });

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure();
    builder.Services.AddFluentValidationAutoValidation();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseHttpsRedirection();
    app.MapControllers();
    app.MapHealthChecks("/health");

    Log.Information("Starting Unit Conversion API");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;