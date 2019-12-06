using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.WebJobs.Host.Queues;
using PublicAPI.Functions.Operations;
using System.Diagnostics;
using Microsoft.ApplicationInsights.Extensibility;

[assembly: FunctionsStartup(typeof(PublicAPI.Functions.Startup))]

namespace PublicAPI.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process));

            builder.Services.AddSingleton<IQueueProcessorFactory>(sp => {

                //var tc = sp.GetService<TelemetryClient>();

                ////TODO: remove once App insights works locally
                //if (string.IsNullOrEmpty(tc.InstrumentationKey) && Debugger.IsAttached)
                //{
                //    tc.InstrumentationKey = "fcb0f03a-5906-4b13-9afb-de4f80999f9d";
                //}

                return new QueueProcessorFactory(sp.GetService<CloudBlobClient>(), sp.GetService<TelemetryConfiguration>());

            });
            builder.Services.AddSingleton(s => new HttpClient());

            builder.Services.AddSingleton(s => storageAccount);
            builder.Services.AddSingleton(s => s.GetService<CloudStorageAccount>().CreateCloudBlobClient());
            builder.Services.AddSingleton(s => s.GetService<CloudStorageAccount>().CreateCloudQueueClient());
        }
    }
}