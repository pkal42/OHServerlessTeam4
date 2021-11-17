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

namespace OpenHack
{
    public  class Rating
    {
        
        [JsonProperty("id")]
        public Guid RatingId { get; set;}
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string LocationName { get; set; }
        
        [JsonProperty("rating")]
        public int RatingScore { get; set; }

        public string UserNotes { get; set;}

    }
}