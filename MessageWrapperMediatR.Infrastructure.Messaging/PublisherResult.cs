namespace MessageWrapperMediatR.Infrastructure.Messaging
{
    public class PublisherResult
    {
        public string MessageId { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }
    }
}
