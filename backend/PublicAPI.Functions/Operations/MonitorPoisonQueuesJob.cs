namespace PublicAPI.Functions.Operations
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;
    using Microsoft.Azure.Storage.Queue;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Microsoft.ApplicationInsights;

    public class MonitorPoisonQueuesJob
    {
        public MonitorPoisonQueuesJob(CloudQueueClient queueClient, TelemetryClient telemetryClient)
        {
            this.queueClient = queueClient;
            this.telemetryClient = telemetryClient;
        }

        [FunctionName("MonitorPoisonQueues")]
        [Disable("DisableScheduleJobs")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            var queuesToMonitor = new List<CloudQueue>();

            QueueContinuationToken continuationToken = null;

            do
            {
                var segment = await queueClient.ListQueuesSegmentedAsync(continuationToken);
                var poisonQueues = segment.Results.Where(
                          q => q.Name.EndsWith("-poison", StringComparison.InvariantCultureIgnoreCase));
                queuesToMonitor.AddRange(poisonQueues);

                continuationToken = segment.ContinuationToken;
            }
            while (continuationToken != null);

            var totalQueueLength = 0.0;
            foreach (var queue in queuesToMonitor)
            {
                await queue.FetchAttributesAsync();
                var queueLength = queue.ApproximateMessageCount;

                telemetryClient.TrackMetric($"Poisonqueue length - {queue.Name}", (double)queueLength);
                log.LogInformation($"Queue: {queue.Name} (Items: {queueLength})");

                totalQueueLength += (double)queueLength;
            }

            telemetryClient.TrackMetric($"Poisonqueue length - Total", totalQueueLength);
        }

        CloudQueueClient queueClient;
        TelemetryClient telemetryClient;
    }
}
