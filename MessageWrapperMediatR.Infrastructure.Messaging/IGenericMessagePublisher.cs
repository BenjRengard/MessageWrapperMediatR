using MessageWrapperMediatR.Core.Models;

namespace MessageWrapperMediatR.Infrastructure.Messaging
{
    public interface IGenericMessagePublisher
    {
        /// <summary>
        /// Publie le message dans les bus définis.
        /// </summary>
        /// <typeparam name="T">Type du contenu du message</typeparam>
        /// <param name="message">Enveloppe du message</param>
        /// <returns>Résultat des envois. False si un envoi est KO.</returns>
        Task<PublisherResult> PublishMessageAsync<T>(PublisherMessageEnveloppeBase<T> message);

        /// <summary>
        /// Publie un message dont l'enveloppe est déjà formée. Ne pas utiliser dans le code en direct. Uniquement encapsulé dans la commande associée.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enveloppe"></param>
        /// <returns></returns>
        Task<PublisherResult> DirectPublishToBusAsync<T>(PublisherMessageEnveloppeBase<T> enveloppe);

        List<PublisherMessageEnveloppeBase<T>> BuildEnveloppes<T>(PublisherMessageEnveloppeBase<T> enveloppeBase);

        PublisherMessageEnveloppeBase<object> BuildEnveloppeFromMetadata(MessageBusEnum messageBus, string endpoint, string messageJson, string routingKey = null);
    }
}
