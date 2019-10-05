namespace PublicAPI.Tests
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public class CatalogIndexReader
    {
        public CatalogIndexReader(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<CatalogIndex> ReadUrl(string catalogIndexUrl)
        {
            var response = await httpClient.GetAsync(catalogIndexUrl);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            return await System.Text.Json.JsonSerializer.DeserializeAsync<CatalogIndex>(responseStream);
        }

        readonly HttpClient httpClient;
    }
}
