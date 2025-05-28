using Consul;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;
using swarm_microservices.gateway.Configurations;
using swarm_microservices.gateway.Services;

namespace swarm_microservices.gateway.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddDependencies(
        this IServiceCollection services,
        WebApplicationBuilder builder
    )
    {
        if (builder.Environment.IsStaging())
        {
            builder.Services.AddConsul(builder.Configuration.GetServiceConfig());
        }

        builder
            .Configuration.SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile(
                $"appsettings.{builder.Environment.EnvironmentName}.json",
                optional: false,
                reloadOnChange: true
            )
            .AddJsonFile(
                $"ocelot.{builder.Environment.EnvironmentName}.json",
                optional: false,
                reloadOnChange: true
            )
            .AddEnvironmentVariables();

        builder.Services.AddOcelot().AddConsul();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHostedService<ServiceDiscoveryHostedService>();
        builder.Services.AddHealthChecks();

        return services;
    }

    public static ServiceConfig GetServiceConfig(this IConfiguration configuration)
    {
        if (configuration is not null)
        {
            var serviceConfig = new ServiceConfig
            {
                Id = configuration.GetValue<string>("Consul:Id"),
                Name = configuration.GetValue<string>("Consul:Name"),
                Address = configuration.GetValue<string>("Consul:Address"),
                Port = configuration.GetValue<int>("Consul:Port"),
                DiscoveryAddress = configuration.GetValue<Uri>("Consul:DiscoveryAddress"),
                HealthCheckEndPoint = configuration.GetValue<string>("Consul:HealthCheckEndPoint"),
            };

            return serviceConfig;
        }

        throw new ArgumentNullException(nameof(configuration));
    }

    public static void AddConsul(this IServiceCollection services, ServiceConfig serviceConfig)
    {
        ArgumentNullException.ThrowIfNull(serviceConfig);

        var consulClient = new ConsulClient(config =>
        {
            config.Address = serviceConfig.DiscoveryAddress;
        });

        services.AddSingleton(serviceConfig);
        services.AddSingleton<IConsulClient, ConsulClient>(_ => consulClient);
        services.AddHostedService<ServiceDiscoveryHostedService>();
    }
}
