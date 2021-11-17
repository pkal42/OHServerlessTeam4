using System;
using System.IO;
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

            string reqBody = await new StreamReader(req.Body).ReadToEndAsync();
            Rating rating = JsonConvert.DeserializeObject<Rating>(reqBody);

            if (rating.RatingScore >=0 && rating.RatingScore <=5) {
               rating.RatingId = Guid.NewGuid();
               rating.TimeStamp = DateTime.Now;
            
               await ratingsout.AddAsync(rating);
            
                return new CreatedResult("/rating", rating);
            }

            return new BadRequestResult();
        }
    }
}
