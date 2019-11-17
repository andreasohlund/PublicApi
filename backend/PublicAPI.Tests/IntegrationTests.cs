namespace PublicAPI.Tests
{
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Azure.Storage.Queue;
    using Microsoft.Azure.WebJobs.Extensions.Timers;
    using NUnit.Framework;
    using PublicAPI.Functions;
    using PublicAPI.Functions.Operations;
    using PublicAPI.Messages;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class IntegrationTests
    {
        [Test]
        public async Task IndexNuGetPackages()
        {
            var collector = new TestCollector<Messages.ExtractPackageAPI>();
            var function = new IndexNuGetPackagesJob(httpClient, cloudBlobClient);

            await function.Run(new Microsoft.Azure.WebJobs.TimerInfo(new FakeTimerSchedule(), new ScheduleStatus()), new TestLogger(), collector);

            Assert.True(collector.Items.Any());
        }

        [Test]
        public async Task RetryPoisonMessages()
        {
            var function = new RetryPoisonMessagesJob(cloudQueueClient);

            await function.Run(new Microsoft.Azure.WebJobs.TimerInfo(new FakeTimerSchedule(), new ScheduleStatus()), new TestLogger());
        }


        [TestCase("NServiceBus", "7.1.0")]
        [TestCase("Xenko.Core.Design", "3.1.0.1-beta02-0752+ge8c8e4af")]
        public async Task ExtractPackageAPI(string package, string version)
        {
            var function = new ExtractPackageAPIFunctions(httpClient, cloudBlobClient);
            var message = new ExtractPackageAPI
            {
                PackageId = package,
                PackageVersion = version,
                HasDotNetAssemblies = true
            };

            await function.HandleMessage(message, new TestLogger());
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            httpClient = new HttpClient();
            storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("PublicAPI_UnitTestStorage", EnvironmentVariableTarget.User));

            cloudBlobClient = storageAccount.CreateCloudBlobClient();
            cloudQueueClient = storageAccount.CreateCloudQueueClient();
        }

        CloudStorageAccount storageAccount;
        CloudBlobClient cloudBlobClient;
        CloudQueueClient cloudQueueClient;
        HttpClient httpClient;
    }
}
