using Akka.Event;
using Akka.Actor;
using Akka.Logger.Serilog;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ETL_Metrics.Actors
{
    public class ConsoleSinkFormatter : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger<SerilogLoggingAdapter>();

        public ConsoleSinkFormatter()
        {
            Receive<MessageQueueStats>(FormatMessage);
            Receive<Flush>(HandleFlush);
        }

        #region Messages

        public class Flush
        {
            public List<string> StringValuesToLog { get; }
            public List<int> IntValueToLog { get; }

            public Flush(string customstr101 = "",
                string customstr102 = "", string customstr103 = "", string customstr104 = "", string customstr105 = "",
                int customnb101 = 0, int customnb102 = 0, int customnb103 = 0, int customnb104 = 0, int customnb105 = 0)
            {
                StringValuesToLog = new List<string> { customstr101, customstr102, customstr103, customstr104, customstr105 };
                IntValueToLog = new List<int> { customnb101, customnb102, customnb103, customnb104, customnb105 };
            }
        }

        public class MessageQueueStats
        {
            public MessageQueueStats(string payload)
            {
                Payload = payload;
            }

            public string Payload { get; }
        }


        private class MessageStats
        {
            public int Ack { get; set; }
            public int Deliver { get; set; }
            public int Deliver_get { get; set; }
            public int Publish { get; set; }
            public int Redeliver { get; set; }
        }

        private class RmqStats
        {
            public string Name { get; set; }
            public string Node { get; set; }
            public string State { get; set; }
            public int Consumers { get; set; }
            public MessageStats Message_stats { get; set; }
        }

        #endregion Messages

        /// <summary>
        /// This unfortunately is a PCF to Elastic convention
        /// </summary>
        /// <param name="_"></param>
        private void HandleFlush(Flush _)
        {
            var logMessage = "";
            for (int i = 0; i <= _.StringValuesToLog.Count - 1; i++)
            {
                if (!string.IsNullOrWhiteSpace(_.StringValuesToLog[i]))
                {
                    logMessage += $"customstr10{i + 1}={_.StringValuesToLog[i]} ";
                }
            }
            for (int i = 0; i <= _.IntValueToLog.Count - 1; i++)
            {
                logMessage += $"customnb10{i + 1}={_.IntValueToLog[i]} ";
            }
            _log.Info(logMessage);
        }

        private void FormatMessage(MessageQueueStats _)
        {
            var stat = JsonConvert.DeserializeObject<RmqStats>(_.Payload);

            var flush = new Flush(customstr101: $"{stat.Name}-{stat.Node}",
                       customstr102: "Acknowledged",
                       customstr103: "PushedToConsumer",
                       customstr104: "Published",
                       customstr105: "Redelivered",
                       customnb101: 0,
                       customnb102: stat.Message_stats?.Ack ?? 0,
                       customnb103: stat.Message_stats?.Deliver ?? 0,
                       customnb104: stat.Message_stats?.Publish ?? 0,
                       customnb105: stat.Message_stats?.Redeliver ?? 0);

            Self.Tell(flush);
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new ConsoleSinkFormatter());
        }
    }
}