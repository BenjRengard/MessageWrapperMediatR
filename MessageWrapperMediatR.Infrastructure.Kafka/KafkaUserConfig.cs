namespace MessageWrapperMediatR.Infrastructure.Kafka
{
    public class KafkaUserConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string SslKeystoreLocation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SslKeystorePassword { get; set; }

        /// <summary>
        /// Group Id pour l'application.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Client Id
        /// </summary>
        public string ClientId { get; set; }
    }
}
