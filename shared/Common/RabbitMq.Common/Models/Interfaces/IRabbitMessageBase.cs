using System;

namespace RosGrant.RabbitMq.Data.Models.RabbitMessage
{
    public interface IRabbitMessageBase
    {
        Guid MessageId { get; set; }
    }
}
