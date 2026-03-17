namespace FlexFit.Models
{
    public class DailyCard : MembershipCard
    {
        public string SerialNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public TimeSpan PurchaseTime { get; set; }
        public virtual ICollection<FitnessObject> FitnessObjects { get; set; } = new List<FitnessObject>();
    }
}