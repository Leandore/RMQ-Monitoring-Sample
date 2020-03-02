using Core;
using Serilog;
using Akka.Actor;
using ETL_Metrics.Actors;
using ETL_Metrics.HostedServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Core.RMQ;
using Destructurama;
using Serilog.Formatting.Compact;

namespace ETL_Metrics
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
                    var configuration = hostContext.Configuration;

                    var rabbitInfrastructureConfig = new RabbitInfrastructure();
                    configuration.Bind("RabbitInfrastructure", rabbitInfrastructureConfig);
                    services.AddSingleton(rabbitInfrastructureConfig);

                    services.AddSingleton(provider =>
                    {
                        var serviceScopeFactory = provider.GetService<IServiceScopeFactory>();
                        var actorSystem = ActorSystem.Create("metric-collector", AkkaConfig.Load());
                        actorSystem.AddServiceScopeFactory(serviceScopeFactory);
                        return actorSystem;
                    });

                    services.AddSingleton<MetricCollectorProvider>(provider =>
                    {
                        var actorSystem = provider.GetService<ActorSystem>();
                        return () => actorSystem.ActorOf(MetricCollector.Props(), "metric-collector");
                    });

                    services.AddHostedService<MetricCollectionService>();

                    services.AddLogging(configure => configure.AddSerilog());

                    //var configBuilder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", true).Build();
                    //Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configBuilder).CreateLogger();

                    Log.Logger = new LoggerConfiguration()
                        .Destructure.JsonNetTypes()
                        .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly("@Message like '%custom%'").WriteTo.Console(outputTemplate: "{Message}{NewLine}"))
                        .WriteTo.Logger(lc => lc.Filter.ByExcluding("@Message like '%custom%'").WriteTo.EventCollector("http://splunk:8088", "eef59a78-c203-431b-90c9-943077c9c942"))

                   // .WriteTo.EventCollector("http://splunk:8088", "eef59a78-c203-431b-90c9-943077c9c942", jsonFormatter: new RenderedCompactJsonFormatter()))

                    .CreateLogger();

                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", optional: true);
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                    configApp.AddEnvironmentVariables();
                });

            return builder;
        }
    }
}