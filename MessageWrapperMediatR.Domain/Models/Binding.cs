using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Domain.Models
{
    /// <summary>
    /// A Binding is a class who represents the definition of bind between flux and action.
    /// </summary>
    public class Binding
    {
        /// <summary>
        /// Unique Id of the associate Handler.
        /// </summary>
        public required string HandlerId { get; set; }

        /// <summary>
        /// Exchange in RabbitMq definition.
        /// </summary>
        public string? Exchange { get; set; }

        /// <summary>
        /// Routing Key in RabbitMqDefinition
        /// </summary>
        public string? RoutingKey { get; set; }

        /// <summary>
        /// Associate handler
        /// </summary>
        public Handler Handler { get; set; }
    }
}
