namespace PublicAPI.Functions
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using System.Threading.Tasks;

    public class GetPackageApi
    {
        [FunctionName("GetPackageApi")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous,
            Route = "{package:string}/{id:string}")] string package, string id)
        {
            var message = $"Package: {package}, ID: {id}";
            return (ActionResult)new OkObjectResult(message);
        }
    }
}
