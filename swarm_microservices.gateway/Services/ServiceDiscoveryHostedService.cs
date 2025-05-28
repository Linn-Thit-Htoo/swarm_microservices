using Consul;
using swarm_microservices.gateway.Configurations;

namespace swarm_microservices.gateway.Services
{
    public class ServiceDiscoveryHostedService : IHostedService
    {
        private readonly IConsulClient _client;
        private readonly ServiceConfig _config;
        private AgentServiceRegistration _registration;

        public ServiceDiscoveryHostedService(IConsulClient client, ServiceConfig config)
        {
            _client = client;
            _config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _registration = new AgentServiceRegistration
            {
                ID = _config.Id,
                Name = _config.Name,
                Address = _config.Address,
                Port = _config.Port,
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    Interval = TimeSpan.FromSeconds(10),
                    HTTP = _config.HealthCheckEndPoint,
                    Timeout = TimeSpan.FromSeconds(10),
                },
            };

            // Deregister already registered service
            await _client
                .Agent.ServiceDeregister(_registration.ID, cancellationToken)
                .ConfigureAwait(false);

            // Registers service
            await _client.Agent.ServiceRegister(_registration, cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var registration = new AgentServiceRegistration { ID = _config.Id };

            await _client
                .Agent.ServiceDeregister(_registration.ID, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
