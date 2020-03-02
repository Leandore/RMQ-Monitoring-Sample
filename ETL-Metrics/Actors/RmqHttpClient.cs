using RestSharp;
using Akka.Actor;
using RestSharp.Authenticators;
using Microsoft.Extensions.DependencyInjection;
using Core.RMQ;

namespace ETL_Metrics.Actors
{
    public class RmqHttpClient : ReceiveActor
    {
        private RestClient _client;
        private string RmqHostUrl { get; }
        private string RmqVitualHost{ get; }
        private string RmqUsername { get; }
        private string RmqPassword { get; }

        public RmqHttpClient()
        {
            using (var serviceScope = Context.CreateScope())
            {
                var rmqConfig = serviceScope.ServiceProvider.GetService<RabbitInfrastructure>();
                RmqHostUrl = rmqConfig.RMQHost;
                RmqUsername = rmqConfig.RMQUsername;
                RmqPassword = rmqConfig.RMQPassword;
                RmqVitualHost = rmqConfig.VHost;
            }

            Receive<Init>(_ =>
            {
                _client = new RestClient(RmqHostUrl)
                {
                    Authenticator = new HttpBasicAuthenticator(RmqUsername, RmqPassword)
                };

                Become(ReadyToCollectMetrics);
            });

        }

        #region Message Handlers

        private void HandleCollectMetricMessage(CollectMetric _)
        {
            // will return the last hour's data on queue lengths, with a sample for every minute
            //var request = new RestRequest($"/api/queues/{RmqVitualHost}?lengths_age=3600&lengths_incr=600&msg_rates_age=30000&msg_rates_incr=100", Method.GET);


            // will return the last minute data on queue lengths, with a sample for every ten seconds
            var request = new RestRequest($"/api/queues/{RmqVitualHost}?lengths_age=60&lengths_incr=10", Method.GET);
            var response = _client.Execute(request);

            Sender.Tell(new RmqCollector.ProcessMetrics(response.Content));
        }

        #endregion Message Handlers

        #region Finite states

        private void ReadyToCollectMetrics()
        {
            Receive<CollectMetric>(HandleCollectMetricMessage);
        }

        #endregion Finite states

        #region Messages

        public class Init { }

        public class CollectMetric { }

        #endregion Messages

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new RmqHttpClient());
        }
    }
}
