namespace FlexFit.Models
{
    public class DailyCard : MembershipCard
    {
        public string SerialNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public TimeSpan PurchaseTime { get; set; }
        public int FitnessObjectId { get; set; }
        public FitnessObject FitnessObject { get; set; }
    }
}