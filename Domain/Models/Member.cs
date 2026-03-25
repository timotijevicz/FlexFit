namespace FlexFit.Domain.Models
{
    public class Member : User
    {
        public string JMBG { get; set; }
        public int PenaltyPoints { get; set; }
 
        // public ICollection<Reservation> Reservations { get; set; }
        // public ICollection<PenaltyPoint> PenaltyPointHistory { get; set; }
        // public ICollection<PenaltyCard> PenaltyCards { get; set; }
        public ICollection<SubscriptionCard> SubscriptionCards { get; set; } = new List<SubscriptionCard>();
    }
}