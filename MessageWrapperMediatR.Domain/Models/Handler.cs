namespace MessageWrapperMediatR.Domain.Models
{
    /// <summary>
    /// Definition of a model of Handler. 
    /// A handler is the class who defines how a flux is read and what command is associate.
    /// </summary>
    public class Handler
    {
        /// <summary>
        /// Id of an handler model.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Verify if the handler is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Name of the flux who is readed.
        /// </summary>
        public string Queue { get; set; }

        /// <summary>
        /// How long the messages recevied need to be stored.
        /// </summary>
        public int TimeToLiveInDays { get; set; }

        /// <summary>
        /// Verify if the messages received are stored.
        /// </summary>
        public bool IsStored { get; set; }

        /// <summary>
        /// Name of the associate MediatR Command which is automaticaly throw.
        /// </summary>
        public string AssociateCommand { get; set; }

        /// <summary>
        /// Verify if the handler is define in a permanent state in app parameters or if can be edited and deleted because only exists on DB.
        /// </summary>
        public bool IsPermanent { get; set; }

        /// <summary>
        /// Define the messaging bus for the handle to connect.
        /// </summary>
        public MessageBusEnum BusType { get; set; }

        /// <summary>
        /// Asocciate Binding in a RabbitMq definition.
        /// </summary>
        public List<Binding> Bindings { get; set; } = [];

        public Handler() { }

        public Handler(Handler handler)
        {
            Id = handler.Id;
            IsActive = handler.IsActive;
            Queue = handler.Queue;
            IsStored = handler.IsStored;
            AssociateCommand = handler.AssociateCommand;
            TimeToLiveInDays = handler.TimeToLiveInDays;
            BusType = handler.BusType;
            AddBindings(handler.Bindings);
        }

        public void ModifyHandler(Handler handler)
        {
            IsActive = handler.IsActive;
            Queue = handler.Queue;
            IsStored = handler.IsStored;
            AssociateCommand = handler.AssociateCommand;
            TimeToLiveInDays = handler.TimeToLiveInDays;
            Bindings.Clear();
            AddBindings(handler.Bindings);
        }

        /// <summary>
        /// Add a associate binding into Handler.
        /// </summary>
        /// <param name="binding">Binding to add.</param>
        public void AddBinding(Binding binding)
        {
            if (binding != null)
            {
                binding.HandlerId = this.Id;
                this.Bindings.Add(binding);
            }
        }

        /// <summary>
        /// Add multiple associate bindings into handler.
        /// </summary>
        /// <param name="bindings">List of binding to add.</param>
        public void AddBindings(List<Binding> bindings)
        {
            if (bindings != null && bindings.Count != 0)
            {
                foreach (Binding binding in bindings)
                {
                    this.AddBinding(binding);
                }
            }
        }
    }
}
