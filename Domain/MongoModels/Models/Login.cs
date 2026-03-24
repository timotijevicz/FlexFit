using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlexFit.Domain.MongoModels.Models
{
    public class Login
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string UserId { get; set; } = string.Empty;   
        public string Email { get; set; } = string.Empty;    
        public string Role { get; set; } = "Member";        
        public DateTime Time { get; set; } = DateTime.UtcNow; 
    }
}