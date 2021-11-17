using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace OpenHack
{
    public static class GetRatings
    {
        [FunctionName("GetRatings")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "ratings/{userId}")] HttpRequest req,
            [CosmosDB(
                databaseName: "bfyoc",
                collectionName: "ratings",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "select * from c where c.userId = {userId}"
            )] IEnumerable<Rating> ratings,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function received a GetRatings request.");
            
            if(ratings.Count() > 0) 
            {
                return new OkObjectResult(ratings);
            } 
            else 
            {
                return new NotFoundResult();
            }                        
        }
    }
}
