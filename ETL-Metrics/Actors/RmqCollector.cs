using Akka.Actor;

namespace ETL_Metrics.Actors
{
    public class RmqCollector : ReceiveActor
    {
        private IActorRef _rmqMetricHttpClient;
        public RmqCollector()
        {
            Receive<Init>(_ =>
            {
                Become(Initialised);

                var rmqClientActorName = "rmq-http-client";

                _rmqMetricHttpClient = Context.Child(rmqClientActorName)
                    .GetOrElse(() => Context.ActorOf(RmqHttpClient.Props(), rmqClientActorName));

                _rmqMetricHttpClient.Tell(new RmqHttpClient.Init());
            });
        }

        #region Messages

        public class Init
        { }

        public class Collect
        { }

        public class ProcessMetrics
        {
            public string MetricPayload { get; }
            public ProcessMetrics(string payload)
            {
                MetricPayload = payload;
            }
        }

        #endregion Messages

        #region Finite states

        private void Initialised()
        {
            Receive<Collect>(_ =>
            {
                _rmqMetricHttpClient.Tell(new RmqHttpClient.CollectMetric());
            });

            Receive<ProcessMetrics>(_ =>
            {
                var rmqMetricBuilder = "rmq-metric-builder";

                Context.Child(rmqMetricBuilder)
                .GetOrElse(() => Context.ActorOf(RmqMetricBuilder.Props(), rmqMetricBuilder)).Tell(new RmqMetricBuilder.BuildQueues(_.MetricPayload));
            });
        }

        #endregion Finite states

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new RmqCollector());
        }
    }
}