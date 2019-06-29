using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Kofti.Extensions;
using Kofti.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Kofti.Services
{
    public class ConfigService : IConfigService
    {
        private readonly ILogger<ConfigService> _logger;
        private readonly KoftiOptions _koftiOptions;
        private readonly ConnectionMultiplexer _connection;
        private ImmutableDictionary<string, object> _configurations;

        public ConfigService(IOptions<KoftiOptions> koftiOptions, ILogger<ConfigService> logger)
        {
            if (koftiOptions == null)
            {
                throw new ArgumentNullException(nameof(koftiOptions), "Cannot find kofti options");
            }

            _logger = logger ?? new Logger<ConfigService>(new NullLoggerFactory());

            _koftiOptions = koftiOptions?.Value;
            var configuration = ConfigurationOptions.Parse(_koftiOptions.RedisServers);
            configuration.Password = _koftiOptions.RedisPassword;
            configuration.AllowAdmin = _koftiOptions.RedisAllowAdmin;
            configuration.ClientName = _koftiOptions.ApplicationName;
            //TODO (peacecwz): Discuss to initialize redis connection as singleton
            _connection = ConnectionMultiplexer.Connect(configuration);

            _configurations = new Dictionary<string, object>().ToImmutableDictionary();
        }

        public void InitClient()
        {
            var publisher = _connection.GetSubscriber();
            publisher.Publish(_koftiOptions.OrchestratorName, new KoftiInitMessage
            {
                ApplicationName = _koftiOptions.ApplicationName,
                MachineName = Environment.MachineName
            }.SerializeAsJson());
        }

        public void Load()
        {
            var subscriber = _connection.GetSubscriber();
            subscriber.Subscribe(_koftiOptions.ApplicationName,
                (channel, message) =>
                {
                    if (message.HasValue)
                    {
                        _configurations = message.ToString().DeserializeAs<Dictionary<string, object>>()
                            .ToImmutableDictionary();
                    }
                });
        }

        public void InitServer(Action<KoftiInitMessage> action)
        {
            var subscriber = _connection.GetSubscriber();
            subscriber.Subscribe(_koftiOptions.ApplicationName,
                (channel, message) =>
                {
                    if (message.HasValue)
                    {
                        var initMessage = message.ToString().DeserializeAs<KoftiInitMessage>();
                        action.Invoke(initMessage);
                    }
                });
        }

        public T GetValue<T>(string key, T defaultValue = default)
        {
            try
            {
                return (T) _configurations[key];
            }
            catch (Exception e)
            {
                _logger.LogError("Cannot get config", e);
                InitClient();
                return defaultValue;
            }
        }

        public async Task PublishAsync(string applicationName, Dictionary<string, object> configurations)
        {
            var publisher = _connection.GetSubscriber();
            await publisher.PublishAsync(applicationName, configurations.SerializeAsJson());
        }
    }
}