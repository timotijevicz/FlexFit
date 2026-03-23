namespace FlexFit.Models
{
    public class TimeSlot
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
