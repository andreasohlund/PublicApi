namespace PublicAPI.Functions.Operations
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;
    using Microsoft.Azure.Storage.Queue;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class RetryPoisonMessagesJob
    {
        public RetryPoisonMessagesJob(CloudQueueClient queueClient)
        {
            this.queueClient = queueClient;
        }

        [FunctionName("RetryPoisonMessages")]
        [Disable()]
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

            foreach (var queue in queuesToMonitor)
            {
                log.LogInformation($"Processing {queue.Name}");

                var targetQueue = queueClient.GetQueueReference( queue.Name.Replace("-poison", ""));

                var batchSize = 32;
                var count = 0;
                while (true)
                {
                    var messages = await queue.GetMessagesAsync(32);

                    foreach (var message in messages)
                    {
                        await targetQueue.AddMessageAsync(new CloudQueueMessage(message.AsBytes));

                        queue.DeleteMessage(message);
                    }

                    count += messages.Count();

                    if (messages.Count() < batchSize)
                    {
                        break;
                    }
                }

                if (count > 0)
                {
                    log.LogInformation($"{count} messages returned to {targetQueue.Name}");
                }
            }
        }

        CloudQueueClient queueClient;
    }
}
