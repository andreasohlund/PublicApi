namespace PublicAPI.Functions.Operations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Azure.WebJobs.Host.Executors;
    using Microsoft.Azure.WebJobs.Host.Queues;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Queue;
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights;

    public class StoreFailuresInBlobStorageProcessor : QueueProcessor
    {
        public StoreFailuresInBlobStorageProcessor(
            QueueProcessorFactoryContext context,
            CloudBlobContainer failedMessageStorage,
            TelemetryClient telemetryClient) : base(context)
        {
            this.context = context;
            this.failedMessageStorage = failedMessageStorage;
            this.telemetryClient = telemetryClient;
        }

        public async override Task CompleteProcessingMessageAsync(CloudQueueMessage message,
            FunctionResult result,
            CancellationToken cancellationToken)
        {
            if (result.Succeeded)
            {
                await DeleteMessageAsync(message, cancellationToken);
                return;
            }

            if (message.DequeueCount < MaxDequeueCount)
            {
                await ReleaseMessageAsync(message, result, VisibilityTimeout, cancellationToken);
                return;
            }

            try
            {
                await StoreFailedMessageDetails(message, result.Exception);
            }
            catch (Exception exception)
            {
                context.Logger.LogCritical(exception, "Failed to store failure details in blob storage");
            }

            await DeleteMessageAsync(message, cancellationToken);
        }

        async Task StoreFailedMessageDetails(CloudQueueMessage message, Exception exception)
        {
            var failedMessage = failedMessageStorage.GetBlockBlobReference(message.Id);

            failedMessage.Metadata.Add("queue", HttpUtility.UrlEncode(context.Queue.Name));
            failedMessage.Metadata.Add("message", HttpUtility.UrlEncode(exception.Message));
            failedMessage.Metadata.Add("stacktrace", HttpUtility.UrlEncode(exception.ToString()));
            failedMessage.Metadata.Add("exceptiontype", HttpUtility.UrlEncode(exception.GetType().FullName));

            failedMessage.Properties.ContentType = "text/json";

            var body = message.AsString;

            await failedMessage.UploadTextAsync(body);

            telemetryClient.TrackEvent("MessageProcessingFailed", new Dictionary<string, string>
            {
                { "queue", context.Queue.Name},
                { "messagebody", body},
                { "exceptiontype", exception.GetType().FullName}
            });
        }

        readonly QueueProcessorFactoryContext context;
        readonly CloudBlobContainer failedMessageStorage;
        readonly TelemetryClient telemetryClient;
    }
}