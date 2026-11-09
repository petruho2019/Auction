using Api.Common.Data.Interfaces;
using AuctionApi.Application.Interfaces;
using AuctionApi.Data.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApi.Endpoints;

public sealed class CreateAuctionEndpoint : IEndpoint
{
    public void Register(IEndpointRouteBuilder endpointsBuilder)
    {
        endpointsBuilder.MapPost("/create", CreateAsync)
            .WithName("CreateAuction")
            .WithSummary("Создать аукцион")
            .WithDescription("Создает аукцион с названием товара, датами начала и окончания и необязательным фото.")
            .Accepts<CreateAuctionRequest>("multipart/form-data")
            .Produces<AuctionResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<Created<AuctionResponse>> CreateAsync(
        [FromForm] CreateAuctionRequest request,
        IAuctionService auctionService,
        CancellationToken cancellationToken)
    {
        var auction = await auctionService.CreateAsync(request, cancellationToken);
        return TypedResults.Created($"/api/v1/auctions/{auction.Id}", auction);
    }
}
