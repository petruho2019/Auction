# RabbitMq.Common

`AddRabbitMq` scans the supplied assemblies for implementations of
`IRabbitMqMessageHandler<TMessage>`, registers them as scoped services, and
adds a hosted listener to the application's Generic Host. The listener starts
and stops with the host; a connection is opened only when at least one handler
is found.

```csharp
services.AddRabbitMq(
    configuration.GetSection("RabbitMq"),
    "auction",
    typeof(Program).Assembly,
    typeof(MyMessageHandler).Assembly);
```

Handlers implement `IRabbitMqMessageHandler<TMessage>` and receive a
cancellation token. The message body is deserialized from JSON. The default
durable topic exchange is `integration.events`; publishers should use the
message type's full name as its routing key. Each service gets a durable queue
per message type. Successful deliveries are acknowledged; handler failures
are rejected into the corresponding `.dead-letter` queue.

Configure `HostName`, `Port`, `UserName`, `Password`, `VirtualHost`, and
`ExchangeName` in the `RabbitMq` configuration section. Defaults target a
local RabbitMQ development broker.
