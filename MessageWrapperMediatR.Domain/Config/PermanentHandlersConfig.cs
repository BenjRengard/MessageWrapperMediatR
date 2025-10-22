using MessageWrapperMediatR.Core.Models;

namespace MessageWrapperMediatR.Core.Config
{
    public class PermanentHandlersConfig
    {
        public List<Handler> Handlers { get; set; } = new List<Handler>();

    }
}
