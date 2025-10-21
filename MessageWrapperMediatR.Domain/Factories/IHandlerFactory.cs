using MessageWrapperMediatR.Core.Interfaces;
using MessageWrapperMediatR.Core.Models;

namespace MessageWrapperMediatR.Core.Factories
{
    /// <summary>
    /// Factory of handlers.
    /// </summary>
    public interface IHandlerFactory
    {
        /// <summary>
        /// Create a handler with is definition and the execution method.
        /// </summary>
        /// <param name="handlerDefinition">Definition of handler.</param>
        /// <param name="executionMethod">Method who is executed when a message is received.</param>
        /// <returns>A Dynamic Handler created.</returns>
        IDynamicHandler CreateHandler(Handler handlerDefinition, Func<string, Task> executionMethod);

        /// <summary>
        /// Restart connections of all handlers.
        /// </summary>
        Task RestartConnectionsOfHandlersAsync();
    }
}
