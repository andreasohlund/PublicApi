using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host.Queues;
using PublicAPI.Functions.Operations;

[assembly: FunctionsStartup(typeof(PublicAPI.Functions.Startup))]

namespace PublicAPI.Functions
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process));

            builder.Services.AddSingleton<IQueueProcessorFactory>(sp => new QueueProcessorFactory(sp.GetService<CloudBlobClient>()));
            builder.Services.AddSingleton(s => new HttpClient());

            builder.Services.AddSingleton(s => storageAccount);
            builder.Services.AddSingleton(s => s.GetService<CloudStorageAccount>().CreateCloudBlobClient());
            builder.Services.AddSingleton(s => s.GetService<CloudStorageAccount>().CreateCloudQueueClient());
        }
    }
}