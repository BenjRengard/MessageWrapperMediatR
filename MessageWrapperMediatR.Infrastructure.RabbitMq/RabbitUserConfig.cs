namespace MessageWrapperMediatR.Infrastructure.RabbitMq
{
    /// <summary>
    /// Definition of rabbit user.
    /// </summary>
    public class RabbitUserConfig
    {
        /// <summary>
        /// User name.
        /// </summary>
        public required string User { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        public required string Password { get; set; }
    }
}
