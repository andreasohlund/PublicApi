using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Http;
//using Microsoft.Extensions.Logging;
using System;

[assembly: FunctionsStartup(typeof(PublicAPI.Functions.Startup))]

namespace PublicAPI.Functions
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddHttpClient();
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process));

            builder.Services.AddSingleton(s => storageAccount);
            builder.Services.AddSingleton(s => s.GetService<CloudStorageAccount>().CreateCloudBlobClient());

            //builder.Services.AddSingleton<ILoggerProvider, Nu>();
        }
    }
}