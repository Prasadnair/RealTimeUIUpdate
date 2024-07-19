using System.Net.Http;
using System.Text.Json;
using System.Text;
using DealUpdateService.Models;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddCors(builder =>
//{
//    builder.AddPolicy("AllowSpecificOrigin", policy =>
//    {
//        policy.WithOrigins("http://localhost:4200")
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});
// Add services to the container.
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
var webApiUrl = "https://localhost:7008";

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseCors("");

app.MapPost("/api/updates/deal-status", async (DealStatusUpdateRequest request) =>
{
    if (string.IsNullOrEmpty(request.DealId) || string.IsNullOrEmpty(request.NewStatus))
    {
        return Results.BadRequest("DealId and NewStatus are required.");
    }

    // Update deal status in the database (simulated here)
    await UpdateDealStatusInDatabase(request.DealId, request.NewStatus);

    // Notify the main Web API
    await NotifyWebApi(request.DealId, request.NewStatus);

    return Results.Ok();
});




async Task UpdateDealStatusInDatabase(string dealId, string newStatus)
{
    // Here you would have your logic to update the deal status in the database.
    // Example:
    // var deal = await _dbContext.Deals.FindAsync(dealId);
    // if (deal != null)
    // {
    //     deal.Status = newStatus;
    //     await _dbContext.SaveChangesAsync();
    // }

    // For demonstration, let's just log the update
    System.Console.WriteLine($"Updated deal {dealId} status to {newStatus} in the database.");
}

async Task NotifyWebApi(string dealId, string newStatus)
{
    var update = new { DealId = dealId, NewStatus = newStatus };
    var content = new StringContent(JsonSerializer.Serialize(update), Encoding.UTF8, "application/json");

    var response = await httpClient.PostAsync($"{webApiUrl}/api/deals/status-update", content);
    response.EnsureSuccessStatusCode();

    System.Console.WriteLine($"Notified Web API about deal {dealId} status update to {newStatus}.");
}

app.Run();


