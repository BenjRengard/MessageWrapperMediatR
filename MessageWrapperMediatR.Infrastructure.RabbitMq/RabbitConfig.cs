namespace MessageWrapperMediatR.Infrastructure.RabbitMq
{
    public class RabbitConfig
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public bool SSlEnabled { get; set; }
        public string? SSlServerName { get; set; }
        public string? SSlCertPath { get; set; }
        public int PrefectSize { get; set; }
        public int PrefectCount { get; set; }
        public RabbitUserConfig? Consumer { get; set; }
        public RabbitUserConfig? Admin { get; set; }
        public RabbitUserConfig? Producer { get; set; }
        public string? ApplicationName { get; set; }
        /// <summary>
        /// Le nom de l'exchange pour publier les messages.
        /// </summary>
        public string ExchangeToPublishMsg { get; set; }
    }
}
