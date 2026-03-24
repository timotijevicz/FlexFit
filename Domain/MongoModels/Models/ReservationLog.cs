using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlexFit.Domain.MongoModels.Models
{
    public class ReservationLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public int MemberId { get; set; }
        public int ResourceId { get; set; }
        public int? SqlId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime LogTime { get; set; }
        public string Status { get; set; }
    }
}
