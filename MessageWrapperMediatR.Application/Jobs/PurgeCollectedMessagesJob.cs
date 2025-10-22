using MediatR;
using MessageWrapperMediatR.Application.Commands.MessageCollector.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessageWrapperMediatR.Application.Jobs
{
    public class PurgeCollectedMessagesJob : BackgroundService
    {
        private readonly int _timerInterval;
        private readonly ILogger<PurgeCollectedMessagesJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public PurgeCollectedMessagesJob(ILogger<PurgeCollectedMessagesJob> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            //_timerInterval = configuration.GetValue<int>("JobSettings:PurgeMessagesJob:JobTimer");
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //une instance de PeriodicTimer pour déclenche des événements périodiquement selon un intervalle de temps donné
            using PeriodicTimer timer = new(TimeSpan.FromSeconds(_timerInterval));

            try
            {
                //Cette boucle s'exécute tant que l'application n'est pas en train de s'arrêter
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    using (IServiceScope scope = _serviceProvider.CreateScope())
                    {
                        IMediator mediator = scope.ServiceProvider.GetService<IMediator>();
                        bool result = await mediator.Send(new PurgeMessagesCommand(), stoppingToken);

                        if (result)
                        {
                            _logger.LogInformation("Messages purgés avec succès.");
                        }
                        else
                        {
                            _logger.LogInformation("Aucun message à purger.");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("La tâche s'arrête.");
            }
        }
    }
}
