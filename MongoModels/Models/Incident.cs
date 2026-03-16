using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlexFit.MongoModels.Models
{

        public class Incident
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string? Id { get; set; } 

            public int MemberId { get; set; }
            public int FitnessObjectId { get; set; }
            public DateTime Time { get; set; }
            public string Reason { get; set; }
        }
    
}
