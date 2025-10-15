namespace MessageWrapperMediatR.Infrastructure.IbmMqSeries
{
    public class MqSeriesConfig
    {
        public required string QueueManagerName { get; set; }
        public required string Channel { get; set; }
        public required string ConnectionName { get; set; }
        public int Port { get; set; }
        public int WaitInterval { get; set; }
        public int CleanUpInterval { get; set; }
        public int TimeForResetInMinutes { get; set; }
        public int TimeOfInactivityInMinutes { get; set; }
        public required string User { get; set; }
        public required string Password { get; set; }
        public bool MqSeriesActivated { get; set; } = false;
    }
}
