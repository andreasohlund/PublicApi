namespace PublicAPI.Tests
{
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Azure.WebJobs.Extensions.Timers;
    using NUnit.Framework;
    using PublicAPI.Functions;
    using PublicAPI.Messages;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public partial class IntegrationTests
    {
        [Test]
        public async Task IndexNuGetPackages()
        {
            var collector = new TestCollector<ExtractPackageAPI>();
            var function = new IndexNuGetPackagesJob(httpClient, cloudBlobClient);

            await function.Run(new Microsoft.Azure.WebJobs.TimerInfo(new FakeTimerSchedule(), new ScheduleStatus()),new TestLogger(), collector);

            Assert.True(collector.Items.Any());
        }


        [Test]
        public async Task ExtractPackageAPI()
        {
            var function = new ExtractPackageAPIHandler(httpClient, cloudBlobClient);
            var message = new ExtractPackageAPI
            {
                PackageId = "NServiceBus",
                PackageVersion = "7.1.0"
            };

            await function.Run(message, new TestLogger());
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            httpClient = new HttpClient();
            storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("PublicAPI_UnitTestStorage", EnvironmentVariableTarget.User));

            cloudBlobClient = storageAccount.CreateCloudBlobClient();
        }

        CloudStorageAccount storageAccount;
        CloudBlobClient cloudBlobClient;
        HttpClient httpClient;
    }
}
