using DealControllerService.HubConfig;
using DealControllerService.Models;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(builder => 
{
    builder.AddPolicy("AllowSpecificOrigin",policy => 
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
var app = builder.Build();

app.MapHub<DealStatusHub>("/dealStatusHub");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Define the endpoint for receiving status updates from the Deal update service
app.MapPost("/api/deals/status-update", async (DealStatusUpdate update, 
                                               IHubContext<DealStatusHub> hubContext) =>
{
    await hubContext.Clients.All.SendAsync("ReceiveStatusUpdate", update.DealId, update.NewStatus);
    return Results.Ok();
});
app.UseCors("AllowSpecificOrigin");

app.Run();


