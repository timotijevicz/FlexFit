namespace FlexFit.Models
{
    public class Member : User
    {
        public string JMBG { get; set; }
        public int PenaltyPoints { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
        public ICollection<PenaltyPoint> PenaltyPointHistory { get; set; }
        public ICollection<PenaltyCard> PenaltyCards { get; set; }

        // DODAJ OVO: Ovo je veza sa karticom (koja je Daily ili Subscription)
        public MembershipCard ActiveCard { get; set; }
    }
}