using EasyNetQ;
using RosGrant.RabbitMq.Data.Abstract;
using RosGrant.RabbitMq.Data.Models;
using RosGrant.RabbitMq.Data.Models.RabbitMessage.RequestResponse;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RosGrant.RabbitMq.Abstract
{
    public interface IRabbitHelper
    {
        Task PublishAsync<T>(T message,
            Action<IPublishConfiguration> configure = null, CancellationToken cancellationToken = default)
            where T : class, IRMQBaseModel;

        Task<SubscriptionResult> SubscribeAsync<T>(string subscriptionId, Func<RabbitMessage<T>, CancellationToken, Task> callback,
            Action<ISubscriptionConfiguration> configure = null, CancellationToken cancellationToken = default)
            where T : class, IRMQBaseModel;

        Task<RabbitResponseMessage<TResponse>> RequestAsync<TRequest, TResponse>(RabbitRequestMessage<TRequest> request,
            Action<IRequestConfiguration> configure = null, CancellationToken cancellationToken = default)
            where TResponse : class, IRMQBaseModel
            where TRequest : class, IRMQBaseModel;

        Task<IAsyncDisposable> RespondAsync<TRequest, TResponse>(Func<RabbitRequestMessage<TRequest>, CancellationToken, Task<RabbitResponseMessage<TResponse>>> responder,
            Action<IResponderConfiguration> configure = null, CancellationToken cancellationToken = default)
            where TResponse : class, IRMQBaseModel
            where TRequest : class, IRMQBaseModel;
    }
}