
namespace PublicAPI.Tests
{
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class PackageMetadataReader
    {

        public PackageMetadataReader(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<PackageMetadata> ReadUrl(string packageMetadataUrl, CancellationToken cancellationToken = default)
        {

            var response = await httpClient.GetAsync(packageMetadataUrl, cancellationToken);

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<PackageMetadata>(responseStream, cancellationToken: cancellationToken);
        }

        readonly HttpClient httpClient;
    }
}