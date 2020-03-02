using Akka.Actor;
using Newtonsoft.Json;

namespace ETL_Metrics.Actors
{
    public class RmqQueueMetric : ReceiveActor
    {
        public RmqQueueMetric()
        {
            Receive<MessageQueueStatContent>(HandleMessageQueueStatContent);
            Receive<ConsoleSink>(HandleConsoleSink);
            Receive<SplunkSink>(HandleSplunkSink);
        }

        #region Message Handlers

        public void HandleMessageQueueStatContent(MessageQueueStatContent _)
        {
            // extend these if you need more sinks
            Self.Tell(new ConsoleSink(_.Payload));
            Self.Tell(new SplunkSink(_.Payload));
        }

        public void HandleConsoleSink(ConsoleSink _)
        {
            var consoleSink = "console-sink-formatter";
            var msg = new ConsoleSinkFormatter.MessageQueueStats(_.Payload);
            Context.Child(consoleSink).GetOrElse(() => Context.ActorOf(ConsoleSinkFormatter.Props(), consoleSink)).Tell(msg);
        }

        public void HandleSplunkSink(SplunkSink _)
        {
            var splunkSink = "splunk-sink-formatter";
            var msg = new SplunkSinkFormatter.Flush(_.Payload);
            Context.Child(splunkSink).GetOrElse(() => Context.ActorOf(SplunkSinkFormatter.Props(), splunkSink)).Tell(msg);
        }

        #endregion Message Handlers

        #region Messages
        public class MessageQueueStatContent
        {
            public string Payload { get; }

            public MessageQueueStatContent(string payload)
            {
                Payload = payload;
            }
        }

        public class ConsoleSink
        {
            public string Payload { get; }

            public ConsoleSink(string payload)
            {
                Payload = payload;
            }
        }

        public class SplunkSink
        {
            public string Payload { get; }

            public SplunkSink(string payload)
            {
                Payload = payload;
            }
        }

        public class Init
        {

        }



        #endregion Messages

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new RmqQueueMetric());
        }
    }
}