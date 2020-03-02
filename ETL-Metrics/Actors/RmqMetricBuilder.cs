using Akka.Actor;
using Newtonsoft.Json.Linq;

namespace ETL_Metrics.Actors
{
    public class RmqMetricBuilder : ReceiveActor
    {
        public RmqMetricBuilder()
        {
            Receive<BuildQueues>(_ =>
            {
                var statsCollection = JArray.Parse(_.MetricPayload);
                foreach (var statObj in statsCollection)
                {
                    var message = new RmqQueueMetric.MessageQueueStatContent(statObj.ToString());
                    Context.Child(statObj["name"].ToString()).GetOrElse(() => Context.ActorOf(RmqQueueMetric.Props(), statObj["name"].ToString())).Tell(message);
                }
            });
        }

        public class BuildQueues
        {
            public string MetricPayload { get; }
            public BuildQueues(string payload)
            {
                MetricPayload = payload;
            }
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new RmqMetricBuilder());
        }
    }
}
