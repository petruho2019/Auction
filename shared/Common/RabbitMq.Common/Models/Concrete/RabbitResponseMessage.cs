using RosGrant.RabbitMq.Data.Abstract;
using System;

namespace RosGrant.RabbitMq.Data.Models.RabbitMessage.RequestResponse
{
    public class RabbitResponseMessage<TMessageModel> : IRabbitMessageBase
        where TMessageModel : class, IRMQBaseModel
    {
        public Guid MessageId { get; set; }

        public TMessageModel ResponseData { get; set; }

        public bool IsSuccess { get; set; } = true;

        public string ErrorMessage { get; set; }

        public RabbitResponseMessage() { }

        public RabbitResponseMessage(TMessageModel data, Guid messageId)
        {
            ResponseData = data;
            MessageId = messageId;
        }

        public RabbitResponseMessage(TMessageModel data)
        {
            ResponseData = data;
            MessageId = Guid.NewGuid();
        }
    }
}
