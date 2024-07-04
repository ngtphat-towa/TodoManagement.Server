using Application;
using Persistence;
using Shared;

using WebApi;

var builder = WebApplication.CreateBuilder(args);

#region Layer
builder.Services.AddApplication();
builder.Services.AddPresentation();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddShared();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
