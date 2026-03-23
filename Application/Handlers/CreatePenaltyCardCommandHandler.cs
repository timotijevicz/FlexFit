using FlexFit.Application.Commands;
using FlexFit.Models;
using FlexFit.UnitOfWorkLayer;
using MediatR;

namespace FlexFit.Application.Handlers
{
    public class CreatePenaltyCardCommandHandler : IRequestHandler<CreatePenaltyCardCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        public CreatePenaltyCardCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<bool> Handle(CreatePenaltyCardCommand request, CancellationToken cancellationToken)
        {
            var twelveHoursAgo = DateTime.UtcNow.AddHours(-12);
            var allPenalties = await _uow.PenaltyCards.GetAllAsync();
            var existingPenalty = allPenalties.Any(c => c.MemberId == request.MemberId 
                                                     && c.FitnessObjectId == request.FitnessObjectId 
                                                     && c.Date >= twelveHoursAgo);

            if (existingPenalty)
            {
               return false; // Cannot issue penalty if one already issued within 12h
            }

            var card = new PenaltyCard
            {
                MemberId = request.MemberId,
                FitnessObjectId = request.FitnessObjectId,
                Date = DateTime.UtcNow,
                Price = request.Price,
                Reason = request.Reason,
                IsCanceled = false
            };

            await _uow.PenaltyCards.AddAsync(card);
            await _uow.SaveAsync();

            return true;
        }
    }
}
