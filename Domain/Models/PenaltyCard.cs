namespace FlexFit.Domain.Models
{
    public class PenaltyCard
    {
        public string Id { get; set; } = string.Empty;
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public int FitnessObjectId { get; set; }
        public FitnessObject FitnessObject { get; set; }
        public DateTime Date { get; set; } // Ovde gledamo onih 12h
        public decimal Price { get; set; }
        public string Reason { get; set; }
        public bool IsCanceled { get; set; } = false;
        public string? CancelReason { get; set; }
        public bool IsPaid { get; set; } = false;
    }
}