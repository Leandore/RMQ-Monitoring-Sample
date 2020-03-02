using Akka.Actor;
using Akka.Event;
using System;

namespace MetricGen.Actors
{
    public class MetricConsumer : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
        public MetricConsumer()
        {
            Receive<GenMetric>(_ =>
            {
                _log.Debug($"Message received - {_.Id}");
                
            });
        }

        public class GenMetric
        {
            public GenMetric(Guid id)
            {
                Id = id;
            }
            public Guid Id { get; }
        }
    }
}
