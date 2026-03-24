using FlexFit.Application.Commands;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using FlexFit.Domain.MongoModels.Models;
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
                
                // MongoDB-only check
                var memberPenalties = await _uow.PenaltyLogs.GetByMemberIdAsync(request.MemberId);
                var existingPenalty = memberPenalties.Any(p => p.FitnessObjectId == request.FitnessObjectId 
                                                         && p.Timestamp >= twelveHoursAgo);

                if (existingPenalty)
                {
                   return false; 
                }

                // Create MongoDB Log directly
                await _uow.PenaltyLogs.AddAsync(new PenaltyLog
                {
                    MemberId = request.MemberId,
                    FitnessObjectId = request.FitnessObjectId,
                    Timestamp = DateTime.UtcNow,
                    Price = (double?)request.Price,
                    Reason = request.Reason,
                    Type = "DailyTicket" // Default type for manual penalty cards
                });

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
