using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Infrastructure.Messaging
{
    /// <summary>
    /// L'enveloppe d'un message à envoyer. Ce message en fonction de son type sera envoyé à un endroit spécifique (RabbitMq, Kafka, etc.)
    /// Le choix de la conversion en JSon... sera fait au niveau de la couche infrastrusture dans le service dédiée
    /// </summary>
    public class PublisherMessageEnveloppeBase<T>
    {
        /// <summary>
        /// Le message que l'on veut transmettre
        /// </summary>
        public T Message { get; }
        /// <summary>
        /// L'identifiant du message qui peut être un GUID ou un identifiant organisationnel
        /// </summary>
        public string MessageId { get; protected set; }
        /// <summary>
        /// L'identifiant du message qui devra être unique. Il peut s'agit d'une rounting key sous Rabbit....
        /// </summary>
        public string MessageType { get; protected set; }
        /// <summary>
        /// Le constructeur par défaut prenant en paramètre le message à envoyer, le type de message et un identifiant de message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="messageId"></param>
        public PublisherMessageEnveloppeBase(T message, string messageType = null, string messageId = null)
        {
            this.Message = message;
            this.MessageId = messageId ?? Guid.NewGuid().ToString();
            this.MessageType = messageType ?? typeof(T).Name;
        }
    }
}
