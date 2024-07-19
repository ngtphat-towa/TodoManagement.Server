using Application;
using Persistence;
using Shared;
using Identity;

using WebApi;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

#region Layer
builder.Services.AddShared();
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddPresentation();
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
app.MapControllers();

// Log application start
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started.");

app.Run();
