using System.Reflection;

namespace MessageWrapperMediatR.Application.Contracts
{
    public class MessageWrapperSystemConfig
    {
        public required Assembly Assembly { get; set; }

        public MessageWrapperSystemConfig(Assembly assembly) => this.Assembly = assembly;
    }
}
