using System.Net;
using TgenNetProtocol;
using TgenSerializer;
using static TgenRemoteCodes.NetworkCodes;

namespace TgenRemoteServer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private Lobby mainServer;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            mainServer = new Lobby(_logger);
            await mainServer.ExecuteAsync(stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            mainServer.StopAsync(cancellationToken);
            _logger.LogInformation("Closing TgenRemoteServer");
            return base.StopAsync(cancellationToken);
        }
    }
}