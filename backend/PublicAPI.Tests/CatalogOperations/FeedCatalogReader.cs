namespace PublicAPI.Tests
{
     using System.Net.Http;
    using System.Threading.Tasks;

    public class FeedCatalogReader
    {
        public async Task<CatalogPage> ReadPageFromNuget(string cursorPosition)
        {
            var client = new HttpClient();
            var url = $"https://api.nuget.org/v3/catalog0/page{cursorPosition}.json";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            return await System.Text.Json.JsonSerializer.DeserializeAsync<CatalogPage>(responseStream);
        }
    }
}
