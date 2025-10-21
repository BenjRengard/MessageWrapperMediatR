using MessageWrapperMediatR.Core.Models;

namespace MessageWrapperMediatR.Application
{
    public class PermanentHandlersConfig
    {
        public List<Handler> Handlers { get; set; } = new List<Handler>();

    }
}
