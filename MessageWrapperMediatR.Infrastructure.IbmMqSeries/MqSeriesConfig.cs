namespace MessageWrapperMediatR.Infrastructure.IbmMqSeries
{
    public class MqSeriesConfig
    {
        public string QueueManagerName { get; set; }
        public string Channel { get; set; }
        public string ConnectionName { get; set; }
        public int Port { get; set; }
        public int WaitInterval { get; set; }
        public int CleanUpInterval { get; set; }
        public int TimeForResetInMinutes { get; set; }
        public int TimeOfInactivityInMinutes { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool MqSeriesActivated { get; set; } = false;
    }
}
