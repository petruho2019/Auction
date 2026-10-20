using Api.Common.Extensions;
using Microsoft.AspNetCore.Routing;

namespace Api.Common.Helpers;


public static class EndpointsHelper
{

    public static IEndpointRouteBuilder RegisterEndpointsFromTypes(IEndpointRouteBuilder builder, Type[] endpointTypes)
    {
        foreach (var end in endpointTypes)
        {
            // builder.Register<end>()
        }

        return builder;
    }
}