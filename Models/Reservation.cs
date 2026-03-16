namespace FlexFit.Models
{
    public enum ReservationStatus { Reserved, Used, NoShow }

    public class Reservation
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
        public DateTime Date { get; set; }
        public ReservationStatus Status { get; set; }
    }
}