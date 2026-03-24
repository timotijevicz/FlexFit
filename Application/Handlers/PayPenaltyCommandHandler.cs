using FlexFit.Application.Commands;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using MediatR;

namespace FlexFit.Application.Handlers
{
    public class PayPenaltyCommandHandler : IRequestHandler<PayPenaltyCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        public PayPenaltyCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<bool> Handle(PayPenaltyCommand request, CancellationToken cancellationToken)
        {
            var card = await _uow.PenaltyCards.GetByIdAsync(request.PenaltyId);
            if (card == null || card.IsCanceled || card.IsPaid) return false;

            card.IsPaid = true;
            await _uow.PenaltyCards.UpdateAsync(card);

            return true;

            return true;
        }
    }
}
