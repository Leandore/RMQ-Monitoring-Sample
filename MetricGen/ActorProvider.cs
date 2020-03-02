using Akka.Actor;

namespace MetricGen
{
    public delegate IActorRef MetricGeneratorActorProvider();
    public delegate IActorRef MetricConsumerActorProvider();
}
