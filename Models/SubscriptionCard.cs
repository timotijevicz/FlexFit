namespace FlexFit.Models
{
    public class SubscriptionCard : MembershipCard
    {
        public DateTime? ValidFrom { get; set; }   // Nullable, popunjava se kada korisnik aktivira
        public DateTime? ValidTo { get; set; }     // Nullable
        public bool PersonalTrainer { get; set; }

        // Veza sa FitnessObjects (1 ili više)
        public ICollection<FitnessObject> FitnessObjects { get; set; } = new List<FitnessObject>();
    }
}