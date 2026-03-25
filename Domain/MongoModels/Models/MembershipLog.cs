using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlexFit.Domain.MongoModels.Models
{
    public class MembershipLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string CardNumber { get; set; }
        public string Action { get; set; } 
        public DateTime NewExpiryDate { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
