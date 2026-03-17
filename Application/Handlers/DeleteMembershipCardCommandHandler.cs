using FlexFit.Application.Commands;
using FlexFit.UnitOfWorkLayer;
using MediatR;

namespace FlexFit.Application.Handlers
{
    public class DeleteMembershipCardCommandHandler : IRequestHandler<DeleteMembershipCardCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        public DeleteMembershipCardCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<bool> Handle(DeleteMembershipCardCommand request, CancellationToken cancellationToken)
        {
            var card = await _uow.MembershipCards.GetByCardNumberAsync(request.CardNumber);
            if (card == null) return false;

            await _uow.MembershipCards.DeleteAsync(card);
            await _uow.SaveAsync();
            return true;
        }
    }
}
