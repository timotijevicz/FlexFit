using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlexFit.MongoModels.Models
{
    public class RateLimitViolation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string IpAddress { get; set; }  
        public string Route { get; set; }       
        public DateTime Timestamp { get; set; } 
    }
}