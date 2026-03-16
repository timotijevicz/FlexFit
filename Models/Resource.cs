namespace FlexFit.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public ResourceType Type { get; set; }
        public ResourceStatus Status { get; set; }
        public int Floor { get; set; }
        public bool IsPremium { get; set; }
        public decimal PremiumFee { get; set; }
        public int FitnessObjectId { get; set; }
        public FitnessObject FitnessObject { get; set; }
    }
    public enum ResourceType
    {
        Kardio,
        Tegovi,
        GrupnaSala
    }

    public enum ResourceStatus
    {
        Slobodan,
        Zauzet,
        UKvaru
    }
}