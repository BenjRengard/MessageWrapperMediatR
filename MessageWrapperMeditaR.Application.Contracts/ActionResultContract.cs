using System.Runtime.Serialization;

namespace MessageWrapperMediatR.Application.Contracts
{
    [DataContract]
    public class ActionResultContract
    {
        /// <summary>
        /// The action is in error.
        /// </summary>
        [DataMember]
        public bool IsError { get; set; }

        /// <summary>
        /// The action is partially succeeded.
        /// </summary>
        [DataMember]
        public bool IsPartialSuccess { get; set; }

        /// <summary>
        /// List of handlers wich are started or stopped.
        /// </summary>
        [DataMember]
        public List<string> HandledHandlers { get; set; }

        public ActionResultContract()
        {
            this.IsError = true;
            this.HandledHandlers = new List<string>();
        }

        /// <summary>
        /// Add handler into ViewModel.
        /// </summary>
        /// <param name="key"></param>
        public void AddHandler(string key)
        {
            this.IsError = false;
            this.HandledHandlers.Add(key);
        }

    }
}
