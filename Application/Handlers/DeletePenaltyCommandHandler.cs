using FlexFit.Application.Commands;
using FlexFit.UnitOfWorkLayer;
using MediatR;

namespace FlexFit.Application.Handlers
{
    public class DeletePenaltyCommandHandler : IRequestHandler<DeletePenaltyCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public DeletePenaltyCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(DeletePenaltyCommand request, CancellationToken cancellationToken)
        {
            if (request.Type == "Card")
            {
                var card = await _uow.PenaltyCards.GetByIdAsync(request.Id);
                if (card == null) return false;
                await _uow.PenaltyCards.DeleteAsync(card);
            }
            else if (request.Type == "Point")
            {
                var point = await _uow.PenaltyPoints.GetByIdAsync(request.Id);
                if (point == null) return false;
                await _uow.PenaltyPoints.DeleteAsync(point);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
