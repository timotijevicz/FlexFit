namespace FlexFit.Models
{
    public class SubscriptionCard : MembershipCard
    {
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool PersonalTrainer { get; set; }
    }
}