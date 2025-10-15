using MessageWrapperMediatR.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Publisher
{
    public class PublisherMessageEnveloppeRabbitMq<T> : PublisherMessageEnveloppeBase<T>
    {
        public string Exchange { get; set; }

        public string RoutingKey { get; private set; }

        /// <summary>
        /// Constructeur de l'enveloppe d'envoi de message RabbitMq.
        /// </summary>
        /// <param name="message">Contenu du message sous format json.</param>
        /// <param name="messageType">Nom du type de message envoyé.</param>
        /// <param name="exchange">Nom de l'exchange dans lequel le message doit être envoyé.</param>
        /// <param name="routingKey">Optionnel. Routing key avec laquelle le message doit être envoyé. Par défaut null. Si null, la valeur est surchargée à partir du messageType.
        /// Si vous souahitez ne pas mettre de routing key, veuillez indiquer une valeur string.Empty.</param>
        /// <param name="messageId">Optionnel. Si vous avez un id de message à fournir, vous pouvez l'insérer. Sinon un id technique sera créé
        /// (cet id ne vaudra que pour l'émission, et non pas pour la réception)</param>
        public PublisherMessageEnveloppeRabbitMq(T message, string messageType, string exchange, string routingKey = null, string messageId = null)
            : base(message, messageType, messageId)
        {
            this.Exchange = exchange;
            this.RoutingKey = routingKey ?? messageType;
        }

    }
}
