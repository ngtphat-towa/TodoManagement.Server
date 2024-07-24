using Application;
using Persistence;
using Shared;
using Identity;

using WebApi;
using WebApi.Middleware;
using Serilog;
using Serilog.Extensions.Logging;

var logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Literate)
    .CreateLogger();


logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));
ILogger<Program> MicrosoftLogger = new SerilogLoggerFactory(logger!)
    .CreateLogger<Program>();

#region Layer
builder.Services.AddShared(MicrosoftLogger);
builder.Services.AddIdentityServices(builder.Configuration, MicrosoftLogger);
builder.Services.AddPersistence(builder.Configuration, MicrosoftLogger);
builder.Services.AddApplication(MicrosoftLogger);
builder.Services.AddPresentation(MicrosoftLogger);
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowOrigin");
}
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();

try
{
    await RegisterIdentity.SeedIdentityData(builder.Services, builder.Configuration, MicrosoftLogger);
    await RegisterPersistence.SeedPersistenceData(builder.Services, builder.Configuration, MicrosoftLogger);
}
catch (Exception ex)
{
    MicrosoftLogger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
}



logger.Information("Web host started");

app.Run();
