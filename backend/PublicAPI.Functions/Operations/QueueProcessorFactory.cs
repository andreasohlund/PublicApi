namespace PublicAPI.Functions.Operations
{
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Azure.WebJobs.Host.Queues;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;

    public class QueueProcessorFactory : IQueueProcessorFactory
    {
        public QueueProcessorFactory(CloudBlobClient cloudBlobClient, TelemetryConfiguration telemetryConfiguration)
        {
            telemetryClient = new TelemetryClient(telemetryConfiguration);
            cloudBlobContainer = cloudBlobClient.GetContainerReference("failedmessages");
        }
        public QueueProcessor Create(QueueProcessorFactoryContext context)
        {
            return new StoreFailuresInBlobStorageProcessor(context, cloudBlobContainer, telemetryClient);
        }

        CloudBlobContainer cloudBlobContainer;
        TelemetryClient telemetryClient;
    }
}