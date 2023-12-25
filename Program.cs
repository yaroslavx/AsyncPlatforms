using AsyncPlatforms.Data;
using AsyncPlatforms.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=RequestDB.db"));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("api/v1/platforms", async (AppDbContext context, ListingRequest listingRequest) =>
{
    if (listingRequest == null) return Results.BadRequest();

    listingRequest.RequestStatus = "ACCEPT";
    listingRequest.EstimatedCompetionTime = "2023-12-26:17:00:00";

    await context.ListingRequests.AddAsync(listingRequest);
    await context.SaveChangesAsync();

    return Results.Accepted($"api/v1/platformstatus/{listingRequest.RequestId}", listingRequest);
});

app.Run();
