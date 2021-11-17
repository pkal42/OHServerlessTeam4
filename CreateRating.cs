using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OpenHack
{
    public static class CreateRating
    {
        private static readonly HttpClient client = new HttpClient();
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "bfyoc",
                collectionName: "ratings",
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<Rating> ratingsout,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function received a create rating request.");

            bool invalidRequest = false;
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Rating rating;
            try
            {
                rating = JsonConvert.DeserializeObject<Rating>(requestBody);
            }
            catch
            {
                return new BadRequestObjectResult("Invalid rating object.");
            }

            string responseMessage = string.Empty;


            HttpResponseMessage response = await client.GetAsync($"https://serverlessohapi.azurewebsites.net/api/GetUser?userId={rating.UserId}");
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                invalidRequest = true;
                responseMessage = $"User '{rating.UserId}' is invalid.";
            }
            else
            {
                log.LogInformation($"User '{rating.UserId}' is valid.");
                response = await client.GetAsync($"https://serverlessohapi.azurewebsites.net/api/GetProduct?productId={rating.ProductId}");
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    invalidRequest = true;
                    responseMessage = $"Product '{rating.ProductId}' is invalid.";
                }
                else
                {
                    log.LogInformation($"Product '{rating.ProductId}' is valid.");
                    if (rating.RatingScore < 0 || rating.RatingScore > 5)
                    {
                        invalidRequest = true;
                        responseMessage = $"Rating '{rating}' value must be an integer between 0 and 5.";
                    }
                    rating.RatingId = Guid.NewGuid();
                    rating.TimeStamp = DateTime.UtcNow;

                    await ratingsout.AddAsync(rating);
                }
            }
            if (invalidRequest)
            {
                return new BadRequestObjectResult(responseMessage);
            }
            else
            {
                return new CreatedResult("/rating", rating);
            }
        }
    }
}
