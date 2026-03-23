namespace FlexFit.Application.DTOs
{
    public class CreateDailyCardDto
    {
        public string CardNumber { get; set; }
        public List<int> FitnessObjectIds { get; set; } = new List<int>();
        public DateTime? PurchaseDate { get; set; }
    }
}
