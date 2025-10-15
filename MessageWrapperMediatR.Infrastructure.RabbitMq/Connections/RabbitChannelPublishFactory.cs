using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Connections
{
    public class RabbitChannelPublishFactory : RabbitConnectionFactoryBase, IRabbitChannelPublishFactory
    {
        private IConnection _connection;
        private IChannel _channel;

        public RabbitChannelPublishFactory(ILogger<RabbitChannelPublishFactory> logger, RabbitConfig rabbitConfig) : base(logger, rabbitConfig)
        { }

        public async Task<IChannel> GetChannelAsync()
        {
            if (_channel != null)
            {
                return _channel;
            }
            try
            {
                //Si la connection n'a pas encore été créé
                _connection ??= await this.GetConnectionAsync(UserTypeEnum.producer);

                if (!_connection.IsOpen)
                {
                    try
                    {
                        _connection.Dispose();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Try to Dispose Rabit Connection failed");
                    }
                    _connection = await this.GetConnectionAsync(UserTypeEnum.producer);
                }
                return await _connection.CreateChannelAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during get channel");
                return null;
            }
        }
    }
}
