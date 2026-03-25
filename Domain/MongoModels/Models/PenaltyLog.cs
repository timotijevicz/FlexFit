using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace FlexFit.Domain.MongoModels.Models
{
    public class PenaltyLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public int MemberId { get; set; }
        public int? FitnessObjectId { get; set; }
        public int? SqlId { get; set; }
        public string Type { get; set; } // Card or Point
        public string Reason { get; set; }
        public double? Price { get; set; }
        public bool IsPaid { get; set; } = false;
        public bool IsCanceled { get; set; } = false;
        public string? CancelReason { get; set; }
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
    }
}
