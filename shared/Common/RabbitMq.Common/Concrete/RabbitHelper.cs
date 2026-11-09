using EasyNetQ;
using Microsoft.Extensions.Hosting;
using RosGrant.RabbitMq.Abstract;
using RosGrant.RabbitMq.Data.Abstract;
using RosGrant.RabbitMq.Data.Models;
using RosGrant.RabbitMq.Data.Models.RabbitMessage.RequestResponse;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RosGrant.RabbitMq.Concrete
{
    internal class RabbitHelper(IBus nodeBus, IHostEnvironment environment) : IRabbitHelper
    {
        private readonly IBus _nodeBus = nodeBus;
        private readonly IHostEnvironment _environment = environment;

        private string GetTopicName() => $"Env.{_environment.EnvironmentName}";

        public async Task PublishAsync<T>(T message,
            Action<IPublishConfiguration> configure = null,
            CancellationToken cancellationToken = default) where T : class, IRMQBaseModel
        {
            var request = new RabbitMessage<T>
            {
                MessageId = Guid.NewGuid(),
                Data = message
            };

            if (configure == null)
            {
                await _nodeBus.PubSub.PublishAsync(request, GetTopicName(), cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await _nodeBus.PubSub.PublishAsync(request, configure, cancellationToken).ConfigureAwait(false);
            }
        }
        public async Task<SubscriptionResult> SubscribeAsync<T>(string subscriptionId, Func<RabbitMessage<T>, CancellationToken, Task> callback,
            Action<ISubscriptionConfiguration> configure = null, CancellationToken cancellationToken = default)
            where T : class, IRMQBaseModel
        {
            if (configure == null)
            {
                return await _nodeBus.PubSub.SubscribeAsync(GetHandlerSubscribeUrl(subscriptionId), callback,
                    x => x.WithTopic(GetTopicName()));
            }
            else
            {
                return await _nodeBus.PubSub.SubscribeAsync(GetHandlerSubscribeUrl(subscriptionId), callback, x =>
                {
                    x.WithTopic(GetTopicName());
                    configure(x);
                });
            }
        }

        public async Task<RabbitResponseMessage<TResponse>> RequestAsync<TRequest, TResponse>(RabbitRequestMessage<TRequest> request,
            Action<IRequestConfiguration> configure = null, CancellationToken cancellationToken = default)
            where TResponse : class, IRMQBaseModel
            where TRequest : class, IRMQBaseModel
        {
            if (configure == null)
            {
                return await _nodeBus.Rpc.RequestAsync<RabbitRequestMessage<TRequest>, RabbitResponseMessage<TResponse>>(
                    request,
                    x =>
                    {
                        x.WithQueueName(GetRequestResponseQueueName(typeof(TRequest), typeof(TResponse)));
                    },
                    cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await _nodeBus.Rpc.RequestAsync<RabbitRequestMessage<TRequest>, RabbitResponseMessage<TResponse>>(
                    request, x =>
                    {
                        x.WithQueueName(GetRequestResponseQueueName(typeof(TRequest), typeof(TResponse)));
                        configure(x);
                    }, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<IAsyncDisposable> RespondAsync<TRequest, TResponse>(Func<RabbitRequestMessage<TRequest>, CancellationToken, Task<RabbitResponseMessage<TResponse>>> responder,
            Action<IResponderConfiguration> configure = null, CancellationToken cancellationToken = default)
            where TResponse : class, IRMQBaseModel
            where TRequest : class, IRMQBaseModel
        {
            if (configure == null)
            {
                return await _nodeBus.Rpc.RespondAsync(responder,
                    x => x.WithQueueName(GetRequestResponseQueueName(typeof(TRequest), typeof(TResponse))), cancellationToken);
            }
            else
            {
                return await _nodeBus.Rpc.RespondAsync(responder, x =>
                {
                    x.WithQueueName(GetRequestResponseQueueName(typeof(TRequest), typeof(TResponse)));
                    configure(x);
                }, cancellationToken);
            }
        }

        private string GetRequestResponseQueueName(Type requestType, Type responseType)
        {
            return $"Env.{_environment.EnvironmentName}.{requestType.FullName}.{responseType.FullName}";
        }

        private string GetHandlerSubscribeUrl(string subscriptionId)
        {
            return $"{subscriptionId}.{_environment.EnvironmentName}";
        }


    }
}