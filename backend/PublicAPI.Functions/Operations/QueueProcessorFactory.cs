namespace PublicAPI.Functions.Operations
{
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Azure.WebJobs.Host.Queues;

    public class QueueProcessorFactory : IQueueProcessorFactory
    {
        public QueueProcessorFactory(CloudBlobClient cloudBlobClient)
        {
            cloudBlobContainer = cloudBlobClient.GetContainerReference("failedmessages");
        }
        public QueueProcessor Create(QueueProcessorFactoryContext context)
        {
            return new StoreFailuresInBlobStorageProcessor(context, cloudBlobContainer);
        }

        CloudBlobContainer cloudBlobContainer;
    }
}