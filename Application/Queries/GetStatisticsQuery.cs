using MediatR;

namespace FlexFit.Application.Queries
{
    public class StatisticsDto
    {
        public int TotalSubscriptionCards { get; set; }
        public int TotalDailyCards { get; set; }
        public decimal TotalPenaltyRevenue { get; set; }
        public int TotalActiveMembers { get; set; }
    }

    public class GetStatisticsQuery : IRequest<StatisticsDto> { }
}
