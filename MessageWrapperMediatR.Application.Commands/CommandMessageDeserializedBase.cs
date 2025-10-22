using MessageWrapperMediatR.Core.Attributes;
using MessageWrapperMediatR.Core.Contracts;
using System.Reflection;
using System.Text.Json;

namespace MessageWrapperMediatR.Application.Commands
{
    public abstract class CommandMessageDeserializedBase<T> : CommandMessageBase
    {
        private T MessageDeserialized { get; set; }

        public override void InitializeData(MessageReceivedData message)
        {
            base.InitializeData(message);
            this.Deserialize();
            this.InsertDeserialzedData();
        }

        private void Deserialize()
        {
            if (!string.IsNullOrWhiteSpace(this.MessageMetadata?.MessageContent))
            {
                this.MessageDeserialized = JsonSerializer.Deserialize<T>(this.MessageMetadata.MessageContent);
            }
        }

        private void InsertDeserialzedData()
        {
            if (this.MessageDeserialized != null)
            {
                PropertyInfo prop = this.GetType().GetProperties().SingleOrDefault(x => x.GetCustomAttribute<DeserializePropertyAttribute>() != null);
                prop?.SetValue(this, this.MessageDeserialized);
            }
        }
    }
}
