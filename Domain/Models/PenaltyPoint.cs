namespace FlexFit.Domain.Models
{
    public class PenaltyPoint
    {
        public string Id { get; set; } = string.Empty;
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public bool IsCanceled { get; set; } = false;
        public string? CancelReason { get; set; }
    }
}