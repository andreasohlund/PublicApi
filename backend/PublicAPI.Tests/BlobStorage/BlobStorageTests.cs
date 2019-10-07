namespace PublicAPI.Tests
{
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using NUnit.Framework;
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class BlobStorageTests
    {

        [Test]
        public async Task SaveJsonBlob()
        {
            var container = cloudBlobClient.GetContainerReference("unit-tests");

            await container.CreateIfNotExistsAsync();

            var blobName = Guid.NewGuid().ToString();

            var blobRef = container.GetBlockBlobReference(blobName);

            blobRef.Metadata.Add("somemetadata", "some-value");
            blobRef.Properties.ContentType = "text/json";

            var content = JsonSerializer.Serialize(new SomeEntity { MyProperty = "test" });

            await blobRef.UploadTextAsync(content);
        }

        class SomeEntity
        {
            public string MyProperty { get; set; }
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("PublicAPI_UnitTestStorage", EnvironmentVariableTarget.User));

            cloudBlobClient = storageAccount.CreateCloudBlobClient();
        }

        CloudStorageAccount storageAccount;
        CloudBlobClient cloudBlobClient;
    }
}
