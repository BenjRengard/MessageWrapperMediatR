using System.Text.Json.Serialization;

namespace MessageWrapperMediatR.Application.Contracts
{
    [JsonConverter(typeof(JsonStringEnumConverter<MessageBusEnumContract>))]
    public enum MessageBusEnumContract
    {
        rabbitmq,
        kafka,
        ibmmqseries
    }
}
