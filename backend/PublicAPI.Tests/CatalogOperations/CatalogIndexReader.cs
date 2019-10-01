namespace PublicAPI.Tests
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public partial class CatalogOperationTests
    {
        public class CatalogIndexReader
        {
            public async Task<CatalogIndex> Read()
            {
                var client = new HttpClient();
                var url = $"https://api.nuget.org/v3/catalog0/index.json";

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using var responseStream = await response.Content.ReadAsStreamAsync();

                return await System.Text.Json.JsonSerializer.DeserializeAsync<CatalogIndex>(responseStream);
            }
        }
    }
}
