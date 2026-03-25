using FlexFit.Application.Commands;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using FlexFit.Domain.MongoModels.Models;
using FlexFit.Infrastructure.Repositories.Interfaces;
using MediatR;
using System.Linq;

namespace FlexFit.Application.Handlers
{
    public class CreatePenaltyCardCommandHandler : IRequestHandler<CreatePenaltyCardCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        public CreatePenaltyCardCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<bool> Handle(CreatePenaltyCardCommand request, CancellationToken cancellationToken)
        {
            try 
            {
                var twelveHoursAgo = DateTime.UtcNow.AddHours(-12);
                
                var memberPenalties = await _uow.PenaltyLogs.GetByMemberIdAsync(request.MemberId);
                var existingPenalty = memberPenalties.Any(p => p.FitnessObjectId == request.FitnessObjectId 
                                                         && p.Date >= twelveHoursAgo);

                if (existingPenalty)
                {
                   return false; 
                }

                var penalty = new PenaltyLog
                {
                    MemberId = request.MemberId,
                    FitnessObjectId = request.FitnessObjectId,
                    Date = DateTime.UtcNow,
                    Price = (double?)request.Price,
                    Reason = request.Reason,
                    Type = "DailyTicket" 
                };

                await _uow.PenaltyLogs.AddAsync(penalty);
                
                try {
                    await _uow.MemberGraph.AssignPenaltyToMemberAsync(penalty.Id, request.MemberId.ToString(), request.Reason);
                } catch (Exception ex) {
                    Console.WriteLine($"[CreatePenaltyCardHandler] Neo4j Sync Error: {ex.Message}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreatePenaltyCardHandler] Error: {ex.Message}");
                return false;
            }
        }
    }
}
