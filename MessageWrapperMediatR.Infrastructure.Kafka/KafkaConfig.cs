using Confluent.Kafka;

namespace MessageWrapperMediatR.Infrastructure.Kafka
{
    public class KafkaConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string BootstrapServers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AutoOffsetReset AutoOffsetReset { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool EnableAutoCommit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool SecurityEnable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SslCaLocation { get; set; }

        public KafkaUserConfig Producer { get; set; } = new KafkaUserConfig();

        public KafkaUserConfig Consumer { get; set; } = new KafkaUserConfig();
    }
}
