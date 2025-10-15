using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
