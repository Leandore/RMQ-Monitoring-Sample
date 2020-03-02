using Akka.Event;
using Akka.Actor;
using Akka.Logger.Serilog;
using Newtonsoft.Json.Linq;

namespace ETL_Metrics.Actors
{
    public class SplunkSinkFormatter : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger<SerilogLoggingAdapter>();

        public SplunkSinkFormatter()
        {
            Receive<Flush>(HandleFlush);
        }

        #region Messages

        public class Flush
        {

            public Flush(string queueStats)
            {
                QueueStats = queueStats;
            }

            public string QueueStats { get; }
        }

        #endregion Messages

        private void HandleFlush(Flush _)
        {
            _log.Info("{@QueueStats}", JObject.Parse(_.QueueStats));
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new SplunkSinkFormatter());
        }
    }
}
