using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Connections
{
    public abstract class RabbitConnectionFactoryBase
    {
        protected readonly ILogger<RabbitConnectionFactoryBase> _logger;
        protected readonly RabbitConfig _rabbitConfig;
        protected readonly SemaphoreSlim _semaphoreGate = new(1);

        protected RabbitConnectionFactoryBase(ILogger<RabbitConnectionFactoryBase> logger, RabbitConfig rabbitConfig)
        {
            _logger = logger;
            _rabbitConfig = rabbitConfig;
        }

        /// <summary>
        /// Get a connection of Rabbit.
        /// </summary>
        /// <returns>IConnection Rabbit.</returns>
        protected async Task<IConnection> GetConnectionAsync(UserTypeEnum typeOfUser)
        {
            var user = this.GetUserByType(typeOfUser);
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitConfig.HostName,
                UserName = user.User,
                Password = user.Password,
                VirtualHost = _rabbitConfig.VirtualHost,
                Ssl = new SslOption
                {
                    Enabled = _rabbitConfig.SSlEnabled,
                    ServerName = _rabbitConfig.SSlServerName ?? string.Empty,
                    AcceptablePolicyErrors =
                        System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch | System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors
                },
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
                Port = _rabbitConfig.Port,
                ClientProvidedName = $"{_rabbitConfig.ApplicationName}-{typeOfUser.ToString()}"
            };

            _logger.LogInformation("Create connection {typeofuser} to Rabbit", typeOfUser);
            return await factory.CreateConnectionAsync();
        }

        private RabbitUserConfig GetUserByType(UserTypeEnum typeOfUser)
        {
            switch (typeOfUser)
            {
                case UserTypeEnum.admin:
                    return _rabbitConfig.Admin;
                case UserTypeEnum.consumer:
                    return _rabbitConfig.Consumer;
                case UserTypeEnum.producer:
                    return _rabbitConfig.Producer;
                default:
                    return null;
            }
        }
    }
}
