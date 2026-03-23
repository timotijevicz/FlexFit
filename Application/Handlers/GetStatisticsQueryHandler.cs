using FlexFit.Application.Queries;
using FlexFit.UnitOfWorkLayer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FlexFit.Data;

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
            var subCards = allCards.OfType<FlexFit.Models.SubscriptionCard>().Count();
            var dailyCards = allCards.OfType<global::DailyCard>().Count();
            
            var allPenalties = await _uow.PenaltyCards.GetAllAsync();
            var totalPenaltyRevenue = allPenalties.Where(p => !p.IsCanceled).Sum(p => p.Price);

            var activeMembers = await _context.Users.CountAsync(u => u.Role == FlexFit.Models.Role.Member);

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
