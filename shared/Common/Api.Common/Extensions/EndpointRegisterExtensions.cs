using Api.Common.Data.Interfaces;
using Microsoft.AspNetCore.Routing;

namespace Api.Common.Extensions;

public static class EndpointRegisterExtensions
{
    extension<T>(IEndpointRouteBuilder builder) where T : IEndpoint, new()
    {
        public IEndpointRouteBuilder Register()
        {
            var endpointsProvider = new T();
            endpointsProvider.Register(builder);
            return builder;
        }
    }
}