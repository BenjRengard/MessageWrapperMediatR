using MessageWrapperMediatR.Domain.Models;

namespace MessageWrapperMediatR.Domain.Interfaces
{
    /// <summary>
    /// Definition of a Dynamic Handler.
    /// It is a handler who read automaticaly a queue or a topic, and generate automaticaly an associate command.
    /// </summary>
    public interface IDynamicHandler : IDisposable
    {
        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous Start operation.</returns>
        Task StartAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous Stop operation.</returns>
        Task StopAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Define the messageing bus for the handle to connect.
        /// </summary>
        MessageBusEnum BusType { get; }

        /// <summary>
        /// Verify if it's active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// the key of handler.
        /// </summary>
        string HandlerKey { get; }

        /// <summary>
        /// Queue.
        /// </summary>
        string QueueFrom { get; }

        /// <summary>
        /// Gets the time-to-live
        /// </summary>
        int TimeToLiveInDays { get; }

        /// <summary>
        /// Gets the command
        /// </summary
        string AssociateCommand { get; }

        /// <summary>
        /// Gets the bindings associated with the handler.
        /// </summary>
        List<Binding> Bindings { get; }

        /// <summary>
        /// Verify if this handler cannot be deleted.
        /// </summary>
        public bool IsPermanent { get; set; }
    }
}
