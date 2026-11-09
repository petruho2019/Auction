using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMq.Common.Configuration;
using RabbitMq.Common.Handlers;
using RabbitMq.Common.Hosting;

namespace RabbitMq.Common;

public static class DependencyInjection
{
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        IConfigurationSection configuration,
        string serviceName,
        params Assembly[] handlerAssemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);
        ArgumentNullException.ThrowIfNull(handlerAssemblies);

        services.AddOptions<RabbitMqOptions>()
            .Bind(configuration)
            .Configure(options => options.ServiceName = serviceName)
            .Validate(options => !string.IsNullOrWhiteSpace(options.HostName), "RabbitMQ host name is required.")
            .Validate(options => options.Port is > 0 and <= 65535, "RabbitMQ port must be between 1 and 65535.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.UserName), "RabbitMQ user name is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.ExchangeName), "RabbitMQ exchange name is required.")
            .ValidateOnStart();

        var registeredHandlers = new HashSet<(Type MessageType, Type HandlerType)>();
        var registeredMessages = new HashSet<Type>();

        foreach (var assembly in handlerAssemblies)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            foreach (var handlerType in assembly.GetTypes().Where(type =>
                         type is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false }))
            {
                foreach (var handlerInterface in handlerType.GetInterfaces().Where(@interface =>
                             @interface.IsGenericType &&
                             @interface.GetGenericTypeDefinition() == typeof(IRabbitMqMessageHandler<>)))
                {
                    var messageType = handlerInterface.GenericTypeArguments[0];
                    if (registeredHandlers.Add((messageType, handlerType)))
                    {
                        services.AddScoped(handlerInterface, handlerType);
                    }

                    if (registeredMessages.Add(messageType))
                    {
                        var registrationType = typeof(RabbitMqHandlerRegistration<>).MakeGenericType(messageType);
                        services.AddSingleton(typeof(IRabbitMqHandlerRegistration), registrationType);
                    }
                }
            }
        }

        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IHostedService, RabbitMqHostedService>());

        return services;
    }
}
