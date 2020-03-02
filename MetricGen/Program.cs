using Akka.Actor;
using Core;
using Core.RMQ;
using MetricGen.Actors;
using MetricGen.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetricGen
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(provider =>
                    {
                        var serviceScopeFactory = provider.GetService<IServiceScopeFactory>();
                        var actorSystem = ActorSystem.Create("rabbit-message-generation", AkkaConfig.Load());
                        actorSystem.AddServiceScopeFactory(serviceScopeFactory);
                        return actorSystem;
                    });

                    services.AddSingleton<MetricGeneratorActorProvider>(provider =>
                    {
                        var actorSystem = provider.GetService<ActorSystem>();
                        return () => actorSystem.ActorOf(Props.Create(() => new MetricGenerator()));
                    });

                    services.AddSingleton<MetricConsumerActorProvider>(provider =>
                    {
                        var actorSystem = provider.GetService<ActorSystem>();
                        return () => actorSystem.ActorOf(Props.Create(() => new MetricConsumer()));
                    });

                    services.ScaffoldRabbitMq();

                    services.AddHostedService<RmqDataGeneration>();
                });

            return builder;
        }
    }

    
}
