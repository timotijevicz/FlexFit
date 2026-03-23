namespace FlexFit.Application.DTOs
{
    public class CreateSubscriptionCardDto
    {
        public string CardNumber { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool PersonalTrainer { get; set; }
        public int? MemberId { get; set; }
    }
}
