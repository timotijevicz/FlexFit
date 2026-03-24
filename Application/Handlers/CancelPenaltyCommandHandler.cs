using FlexFit.Application.Commands;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using MediatR;

namespace FlexFit.Application.Handlers
{
    public class CancelPenaltyCommandHandler : IRequestHandler<CancelPenaltyCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        public CancelPenaltyCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<bool> Handle(CancelPenaltyCommand request, CancellationToken cancellationToken)
        {
            if (request.Type == "Card")
            {
                var card = await _uow.PenaltyCards.GetByIdAsync(request.Id);
                if (card == null || card.IsCanceled) return false;
                
                card.IsCanceled = true;
                card.CancelReason = request.Reason;
                await _uow.PenaltyCards.UpdateAsync(card);
            }
            else if (request.Type == "Point")
            {
                var point = await _uow.PenaltyPoints.GetByIdAsync(request.Id);
                if (point == null || point.IsCanceled) return false;

                point.IsCanceled = true;
                point.CancelReason = request.Reason;
                await _uow.PenaltyPoints.UpdateAsync(point);
            }
            else
            {
                return false;
            }

            // await _uow.SaveAsync(); // No EF changes needed anymore
            return true;
        }
    }
}
