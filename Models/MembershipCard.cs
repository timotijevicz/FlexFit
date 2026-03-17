namespace FlexFit.Models
{
    public class MembershipCard
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public CardType CardType { get; set; }

        public bool IsActive { get; set; }  // 🔥 DODAJ OVO

        public int? MemberId { get; set; }
        public Member Member { get; set; }
    }

    public enum CardType
    {
        Daily,
        Subscription
    }
}