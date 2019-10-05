namespace PublicAPI.Tests
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public class CatalogIndexReader
    {
        public CatalogIndexReader(string catalogIndexUrl)
        {
            this.catalogIndexUrl = catalogIndexUrl;
            httpClient = new HttpClient();
        }

        public async Task<CatalogIndex> Read()
        {
            var response = await httpClient.GetAsync(catalogIndexUrl);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            return await System.Text.Json.JsonSerializer.DeserializeAsync<CatalogIndex>(responseStream);
        }

        readonly string catalogIndexUrl;
        readonly HttpClient httpClient;
    }
}
