using System.Reflection;

namespace MessageWrapperMediatR.Core.Config
{
    public class MessageWrapperSystemConfig
    {
        public required Assembly Assembly { get; set; }

        public MessageWrapperSystemConfig(Assembly assembly) => Assembly = assembly;
    }
}
