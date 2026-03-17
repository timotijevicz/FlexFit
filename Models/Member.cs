namespace FlexFit.Models
{
    public class Member : User
    {
        public string JMBG { get; set; }
        public int PenaltyPoints { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
        public ICollection<PenaltyPoint> PenaltyPointHistory { get; set; }
        public ICollection<PenaltyCard> PenaltyCards { get; set; }

        // SAMO Subscription kartica je obavezna pri registraciji
        public ICollection<SubscriptionCard> SubscriptionCards { get; set; } = new List<SubscriptionCard>();

        // Dnevne karte su opciono povezane
        public ICollection<DailyCard> DailyCards { get; set; } = new List<DailyCard>();
    }
}