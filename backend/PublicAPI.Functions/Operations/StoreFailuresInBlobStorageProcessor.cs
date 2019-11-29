﻿namespace PublicAPI.Functions.Operations
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

    public class StoreFailuresInBlobStorageProcessor : QueueProcessor
    {
        public StoreFailuresInBlobStorageProcessor(QueueProcessorFactoryContext context,
            CloudBlobContainer failedMessageStorage) : base(context)
        {
            this.context = context;
            this.failedMessageStorage = failedMessageStorage;
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

        Task StoreFailedMessageDetails(CloudQueueMessage message, Exception exception)
        {
            var failedMessage = failedMessageStorage.GetBlockBlobReference(message.Id);


            failedMessage.Metadata.Add("queue", HttpUtility.UrlEncode(context.Queue.Name));
            failedMessage.Metadata.Add("message", HttpUtility.UrlEncode(exception.Message));
            failedMessage.Metadata.Add("stacktrace", HttpUtility.UrlEncode(exception.StackTrace));
            
            failedMessage.Properties.ContentType = "text/json";
           
            return failedMessage.UploadTextAsync(message.AsString);
        }

        readonly QueueProcessorFactoryContext context;
        readonly CloudBlobContainer failedMessageStorage;
    }
}