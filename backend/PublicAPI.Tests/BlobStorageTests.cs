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
        public async Task SaveAndReadJsonBlob()
        {
            var container = cloudBlobClient.GetContainerReference("unit-tests");

            await container.CreateIfNotExistsAsync();

            var blobName = Guid.NewGuid().ToString();

            var blobWriteReference = container.GetBlockBlobReference(blobName);

            blobWriteReference.Metadata.Add("somemetadata", "some-value");
            blobWriteReference.Properties.ContentType = "text/json";

            var content = JsonSerializer.Serialize(new SomeEntity { MyProperty = "test" });

            await blobWriteReference.UploadTextAsync(content);

            var blobReadReference = container.GetBlockBlobReference(blobName);

            await blobReadReference.FetchAttributesAsync();

            Assert.AreEqual("text/json", blobReadReference.Properties.ContentType);
            Assert.AreEqual("some-value", blobReadReference.Metadata["somemetadata"]);

            using var readStream = await blobReadReference.OpenReadAsync();

            var entityFromStorage = await JsonSerializer.DeserializeAsync<SomeEntity>(readStream);

            Assert.AreEqual("test", entityFromStorage.MyProperty);
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
