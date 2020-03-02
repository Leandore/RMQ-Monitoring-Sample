using System;
using Akka.Event;
using Akka.Actor;

namespace ETL_Metrics.Actors
{
    public class MetricCollector : ReceiveActor
    {
        private IActorRef _rmqMetricsCollector;

        public MetricCollector()
        {
            Become(Scheduled);
        }

        #region Message Handlers

        private void Scheduled()
        {
            var rmqCollectorActorName = "rmq-collector";
            _rmqMetricsCollector = Context.Child(rmqCollectorActorName).GetOrElse(() => Context.ActorOf(RmqCollector.Props(), rmqCollectorActorName));
            _rmqMetricsCollector.Tell(new RmqCollector.Init());

            Self.Tell(new Schedule());

            Receive<Schedule>(_ =>
            {
                Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(10), Self, new TriggerSchedule(), Self);
            });

            Receive<TriggerSchedule>(_ =>
            {
                Self.Tell(new Schedule());
                _rmqMetricsCollector.Tell(new RmqCollector.Collect());
            });

            Receive<AbortSchedule>(_ =>
            {
                Become(Dormant);
            });
        }

        private void Dormant()
        {
            // let's have a nap
        }

        #endregion Message Handlers

        #region Messages

        public class Init { }

        public class Schedule { }

        public class TriggerSchedule { }

        public class AbortSchedule { }

        #endregion Messages

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new MetricCollector());
        }
    }
}