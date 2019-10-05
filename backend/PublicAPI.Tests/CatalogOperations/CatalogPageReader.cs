namespace PublicAPI.Tests
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public class CatalogPageReader
    {
        public CatalogPageReader(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<CatalogPage> ReadUrl(string catalogPageUrl)
        {
            var response = await httpClient.GetAsync(catalogPageUrl);

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            return await System.Text.Json.JsonSerializer.DeserializeAsync<CatalogPage>(responseStream);
        }

        readonly HttpClient httpClient;
    }
}
