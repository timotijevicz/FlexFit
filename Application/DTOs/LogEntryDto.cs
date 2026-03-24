using System.Text.Json.Serialization;

namespace FlexFit.Application.DTOs
{
    public class LogEntryDto
    {
        public int MemberId { get; set; }
        public int FitnessObjectId { get; set; }
        public string CardNumber { get; set; }
        public string CardStatus { get; set; } 
        public string CardType { get; set; } 
        
        [JsonPropertyName("employeeId")]
        public int EmployeeId { get; set; }
    }
}
