using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace Core.RMQ
{
    public static class RabbitInitialConfiguration
    {
        public static IServiceCollection ScaffoldRabbitMq(this IServiceCollection services)
        {
            var connectionFactory = new ConnectionFactory
            {
                Uri = new Uri($"amqp://guest:guest@rabbit:5672/AIML_DEV1"),
                AutomaticRecoveryEnabled = true,
                RequestedHeartbeat = 30,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(30),
            };

            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            channel.BasicQos(0, 100, false);

            channel.ExchangeDeclare("metrics-exchange", "fanout", true, true);

            var queueArgs = new Dictionary<string, object>
                            {
                                { "x-queue-type", "classic" },
                                { "x-message-ttl", 9600000 }
                            };

            channel.QueueDeclare(queue: $"demo-queue-1", durable: true, exclusive: false, arguments: queueArgs);
            channel.QueueDeclare(queue: $"demo-queue-2", durable: true, exclusive: false, arguments: queueArgs);
            channel.QueueDeclare(queue: $"demo-queue-3", durable: true, exclusive: false, arguments: queueArgs);

            channel.QueueBind($"demo-queue-1", "metrics-exchange", "");
            channel.QueueBind($"demo-queue-2", "metrics-exchange", "");
            channel.QueueBind($"demo-queue-3", "metrics-exchange", "");

            services.AddSingleton(connection);
            services.AddSingleton(channel);

            return services;
        }
    }
}