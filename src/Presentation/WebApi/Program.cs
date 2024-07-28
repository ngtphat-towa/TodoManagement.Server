using Application;

using Identity;

using Persistence;

using Serilog;
using Serilog.Extensions.Logging;

using Shared;

using WebApi;
using WebApi.Middleware;

var logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Literate)
    .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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
}

// Use CORS middleware
app.UseCors("AllowOrigin");

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
