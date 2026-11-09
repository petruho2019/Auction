using RosGrant.RabbitMq.Data.Abstract;

namespace RosGrant.RabbitMq.Data.Models
{
    public class RabbitMessage<TMessageModel> : IRMQBaseModel
        where TMessageModel : class, IRMQBaseModel
    {
        public Guid MessageId { get; set; }
        public TMessageModel Data { get; set; }

        public RabbitMessage() { }

        public RabbitMessage(TMessageModel model)
        {
            Data = model;
        }
    }
}