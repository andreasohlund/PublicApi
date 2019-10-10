namespace PublicAPI.Tests
{
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Azure.WebJobs.Extensions.Timers;
    using Microsoft.Extensions.Logging.Abstractions;
    using NUnit.Framework;
    using PublicAPI.Functions;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class IntegrationTests
    {
        [Test]
        public async Task IndexNuGetPackages()
        {
            var function = new IndexNuGetPackages(httpClient, cloudBlobClient);

            await function.Run(new Microsoft.Azure.WebJobs.TimerInfo(new FakeTimerSchedule(), new ScheduleStatus()),new TestLogger());
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            httpClient = new HttpClient();
            storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("PublicAPI_UnitTestStorage", EnvironmentVariableTarget.User));

            cloudBlobClient = storageAccount.CreateCloudBlobClient();
        }

        class FakeTimerSchedule : TimerSchedule
        {
            public override DateTime GetNextOccurrence(DateTime now)
            {
                return now + TimeSpan.FromSeconds(60);
            }
        }

        CloudStorageAccount storageAccount;
        CloudBlobClient cloudBlobClient;
        HttpClient httpClient;
    }
}
