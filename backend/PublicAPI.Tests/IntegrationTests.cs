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

    public class IntegrationTests
    {
        [Test]
        public async Task IndexNuGetPackages()
        {
            var collector = new TestCollector<ExtractPackageAPI>();
            var function = new IndexNuGetPackagesJob(httpClient, cloudBlobClient);

            await function.Run(new Microsoft.Azure.WebJobs.TimerInfo(new FakeTimerSchedule(), new ScheduleStatus()), new TestLogger(), collector);

            Assert.True(collector.Items.Any());
        }


        [TestCase("NServiceBus", "7.1.0")]
        [TestCase("Xenko.Core.Design", "3.1.0.1-beta02-0752+ge8c8e4af")]
        public async Task ExtractPackageAPI(string package, string version)
        {
            var function = new ExtractPackageAPIHandler(httpClient, cloudBlobClient);
            var message = new ExtractPackageAPI
            {
                PackageId = package,
                PackageVersion = version,
                HasDotNetAssemblies = true
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
