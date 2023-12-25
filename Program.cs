using AsyncPlatforms.Data;
using AsyncPlatforms.Dtos;
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

app.MapGet("api/v1/platforms/{requestId}", (AppDbContext context, string requestId) =>
{
    var listingRequest = context.ListingRequests.FirstOrDefault(lr => lr.RequestId == requestId);
    if (listingRequest == null) return Results.NotFound();

    ListingStatus listingStatus = new ListingStatus
    {
        RequestStatus = listingRequest.RequestStatus,
        ResourceURL = String.Empty
    };

    if (listingRequest.RequestStatus.ToUpper() == "COMPLETE")
    {
        listingStatus.ResourceURL = $"api/v1/platforms/{Guid.NewGuid().ToString()}";
        return Results.Ok(listingStatus);
    }

    listingStatus.EstimatedCompetionTime = "2023-12-26:19:00:00";
    return Results.Ok(listingStatus);
});

app.Run();
