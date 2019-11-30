
namespace PublicAPI.Functions.Operations
{
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Azure.WebJobs.Host.Queues;
    using Microsoft.ApplicationInsights;

    public class QueueProcessorFactory : IQueueProcessorFactory
    {
        private readonly TelemetryClient telemetryClient;

        public QueueProcessorFactory(CloudBlobClient cloudBlobClient, TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
            cloudBlobContainer = cloudBlobClient.GetContainerReference("failedmessages");
        }
        public QueueProcessor Create(QueueProcessorFactoryContext context)
        {
            return new StoreFailuresInBlobStorageProcessor(context, cloudBlobContainer, telemetryClient);
        }

        CloudBlobContainer cloudBlobContainer;
    }
}