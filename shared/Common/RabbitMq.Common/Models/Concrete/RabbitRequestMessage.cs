using RosGrant.RabbitMq.Data.Abstract;
using System;

namespace RosGrant.RabbitMq.Data.Models.RabbitMessage.RequestResponse
{
    public class RabbitRequestMessage<TMessageModel> : IRabbitMessageBase
        where TMessageModel : class, IRMQBaseModel
    {
        public Guid MessageId { get; set; }

        public TMessageModel RequestData { get; set; }

        public RabbitRequestMessage() { }

        public RabbitRequestMessage(TMessageModel data, Guid messageId)
        {
            RequestData = data;
            MessageId = messageId;
        }

        public RabbitRequestMessage(TMessageModel data)
        {
            RequestData = data;
            MessageId = Guid.NewGuid();
        }
    }
}
