using System.Text.Json.Serialization;

namespace FlexFit.Models
{
    [JsonDerivedType(typeof(DailyCard), typeDiscriminator: "daily")]
    [JsonDerivedType(typeof(SubscriptionCard), typeDiscriminator: "subscription")]
    public class MembershipCard
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
       
        public bool IsActive { get; set; }  

        public int? MemberId { get; set; }
        public Member? Member { get; set; }
    }

   
}