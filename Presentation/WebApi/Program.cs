using Application;
using Persistence;
using Shared;
using Identity;

using WebApi;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

#region Layer
builder.Services.AddPresentation();
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddShared();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

// Log application start
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started.");

app.Run();
