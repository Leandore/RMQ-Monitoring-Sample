using Akka.Actor;
using MetricGen.Actors;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetricGen.HostedServices
{
    public class RmqDataGeneration : IHostedService
    {
        private readonly IModel _channel;
        private IActorRef _consumerActor;
        private IActorRef _producerActor;

        public RmqDataGeneration(IModel channel, MetricGeneratorActorProvider producerActor, MetricConsumerActorProvider consumerActor)
        {
            _channel = channel;
            _producerActor = producerActor();
            _consumerActor = consumerActor();
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            var metricsConsumer = new EventingBasicConsumer(_channel);
            metricsConsumer.Received += (ch, ea) =>
            {
                Thread.Sleep(150);
                _channel.BasicAck(ea.DeliveryTag, false);
                var msg = JsonConvert.DeserializeObject<MetricConsumer.GenMetric>(Encoding.UTF8.GetString(ea.Body));
                _consumerActor.Tell(msg);
            };
            _channel.BasicConsume($"demo-queue-1", false, metricsConsumer);

            var metricsConsumer2 = new EventingBasicConsumer(_channel);
            metricsConsumer2.Received += (ch, ea) =>
            {
                Thread.Sleep(250);
                _channel.BasicAck(ea.DeliveryTag, false);
                var msg = JsonConvert.DeserializeObject<MetricConsumer.GenMetric>(Encoding.UTF8.GetString(ea.Body));
                _consumerActor.Tell(msg);

            };
            _channel.BasicConsume($"demo-queue-2", false, metricsConsumer2);

            var metricsConsumer3 = new EventingBasicConsumer(_channel);
            metricsConsumer3.Received += (ch, ea) =>
            {
                Thread.Sleep(350);
                _channel.BasicAck(ea.DeliveryTag, false);

                var msg = JsonConvert.DeserializeObject<MetricConsumer.GenMetric>(Encoding.UTF8.GetString(ea.Body));
                _consumerActor.Tell(msg);
            };
            _channel.BasicConsume($"demo-queue-3", false, metricsConsumer3);

            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            _consumerActor.Tell(PoisonPill.Instance);
            _producerActor.Tell(PoisonPill.Instance);

            return Task.CompletedTask;
        }
    }
}
