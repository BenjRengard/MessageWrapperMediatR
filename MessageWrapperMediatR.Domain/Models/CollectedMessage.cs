namespace MessageWrapperMediatR.Domain.Models
{
    /// <summary>
    /// Collected message from the message Wrapper.
    /// </summary>
    public class CollectedMessage
    {
        /// <summary>
        /// Id in GUID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Raw received message.
        /// </summary>
        public string RawMessage { get; set; }

        /// <summary>
        /// Queue of origin.
        /// </summary>
        public string FromQueue { get; set; }

        /// <summary>
        /// Reception date.
        /// </summary>
        public DateTimeOffset ReceptionDate { get; set; }

        /// <summary>
        /// Verify if the message is handled.
        /// </summary>
        public bool IsHandled { get; set; }

        /// <summary>
        /// Time to live in days.
        /// </summary>
        public int TimeToLiveInDays { get; set; }

        /// <summary>
        /// Associate command of the message when it was received.
        /// </summary>
        public string AssociateCommand { get; set; }

        public CollectedMessage()
        {

        }

        public CollectedMessage(string messageContent, Handler handler)
        {
            FromQueue = handler.Queue;
            ReceptionDate = DateTimeOffset.UtcNow;
            Id = Guid.NewGuid();
            IsHandled = false;
            RawMessage = messageContent;
            TimeToLiveInDays = handler.TimeToLiveInDays;
            AssociateCommand = handler.AssociateCommand;
        }
    }
}
