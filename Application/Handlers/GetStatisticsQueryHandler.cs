using FlexFit.Application.Queries;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FlexFit.Infrastructure.Data;

namespace FlexFit.Application.Handlers
{
    public class GetStatisticsQueryHandler : IRequestHandler<GetStatisticsQuery, StatisticsDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly AppDbContext _context;

        public GetStatisticsQueryHandler(IUnitOfWork uow, AppDbContext context)
        {
            _uow = uow;
            _context = context;
        }

        public async Task<StatisticsDto> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
        {
            var allCards = await _uow.MembershipCards.GetAllAsync();
            var subCards = allCards.OfType<FlexFit.Domain.Models.SubscriptionCard>().Count();
            var dailyCards = allCards.OfType<global::DailyCard>().Count();
            
            var allPenalties = await _uow.PenaltyLogs.GetAllAsync();
            var totalPenaltyRevenue = allPenalties.Sum(p => (decimal)(p.Price ?? 0.0));

            var activeMembers = await _context.Users.CountAsync(u => u.Role == FlexFit.Domain.Models.Role.Member);

            return new StatisticsDto
            {
                TotalSubscriptionCards = subCards,
                TotalDailyCards = dailyCards,
                TotalPenaltyRevenue = totalPenaltyRevenue,
                TotalActiveMembers = activeMembers
            };
        }
    }
}
