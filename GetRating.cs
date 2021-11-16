using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenHack
{
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ratings/{ratingId}")] HttpRequest req,
            [CosmosDB(
                databaseName: "bfyoc",
                collectionName: "ratings",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "select * from c where c.id = {ratingId}"
            )] IEnumerable<Ratings> ratings,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function received a GetRating request.");
            
            foreach (Ratings rating in ratings)
            {
                log.LogInformation(JsonConvert.SerializeObject(rating).ToString());
                return new OkObjectResult(rating);
            }
            return new OkResult();
        }
    }
}
