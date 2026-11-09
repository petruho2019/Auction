using AuctionApi.Application.Interfaces;
using AuctionApi.Application.Mappers;
using AuctionApi.Data.Dtos;
using AuctionApi.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace AuctionApi.Application.Services;

public sealed class AuctionService(
    AuctionDbContext dbContext,
    IWebHostEnvironment environment) : IAuctionService
{
    public async Task<AuctionResponse> CreateAsync(
        CreateAuctionRequest request,
        CancellationToken cancellationToken)
    {
        string? photoPath = null;
        string? physicalPhotoPath = null;

        try
        {
            if (request.Photo is { Length: > 0 } photo)
            {
                var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid():N}{extension}";
                var photoDirectory = Path.Combine(
                    environment.ContentRootPath,
                    "wwwroot",
                    "auction-photos");
                Directory.CreateDirectory(photoDirectory);
                physicalPhotoPath = Path.Combine(photoDirectory, fileName);
                photoPath = $"/auction-photos/{fileName}";

                await using var fileStream = new FileStream(
                    physicalPhotoPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None);
                await photo.CopyToAsync(fileStream, cancellationToken);
            }

            var auction = AuctionMapper.ToEntity(request, photoPath);

            dbContext.Auctions.Add(auction);
            await dbContext.SaveChangesAsync(cancellationToken);

            return AuctionMapper.ToResponse(auction);
        }
        catch
        {
            if (physicalPhotoPath is not null && File.Exists(physicalPhotoPath))
            {
                File.Delete(physicalPhotoPath);
            }

            throw;
        }
    }
}
