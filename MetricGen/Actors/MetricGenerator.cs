using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace MetricGen.Actors
{
    public class MetricGenerator : ReceiveActor
    {
        public MetricGenerator()
        {
            Receive<GenMetric>(_ =>
            {
                using (var serviceScope = Context.CreateScope())
                {
                    var channel = serviceScope.ServiceProvider.GetService<IModel>();
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_));
                    channel.BasicPublish(exchange: "metrics-exchange", routingKey: "", body: body);
                }
            });

            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(10), Self, new GenMetric(), Self);
        }

        #region Messages
        public class GenMetric
        {
            public Guid Id => Guid.NewGuid();
        }
        #endregion Messages
    }
}