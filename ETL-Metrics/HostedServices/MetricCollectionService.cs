using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace ETL_Metrics.HostedServices
{
    public class MetricCollectionService : IHostedService
    {
        public MetricCollectionService(MetricCollectorProvider createCollector)
        {
            createCollector();
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}