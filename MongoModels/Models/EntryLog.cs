using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlexFit.MongoModels.Models
{
    public class EntryLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } 

        public int MemberId { get; set; }
        public int EmployeeId { get; set; }
        public int FitnessObjectId { get; set; }
        public DateTime Time { get; set; }
        public string CardStatus { get; set; }
        public bool Incident { get; set; }
    }
}