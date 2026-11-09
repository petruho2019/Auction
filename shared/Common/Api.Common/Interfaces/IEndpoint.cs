using Microsoft.AspNetCore.Routing;

namespace Api.Common.Data.Interfaces;

public interface IEndpoint
{
    void Register(IEndpointRouteBuilder endpointsBuilder);
}