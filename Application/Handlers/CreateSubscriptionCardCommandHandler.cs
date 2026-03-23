using FlexFit.Application.Commands;
using FlexFit.Models;
using FlexFit.UnitOfWorkLayer;
using MediatR;

namespace FlexFit.Application.Handlers
{
    public class CreateSubscriptionCardCommandHandler : IRequestHandler<CreateSubscriptionCardCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        public CreateSubscriptionCardCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<bool> Handle(CreateSubscriptionCardCommand request, CancellationToken cancellationToken)
        {
            var existingCard = await _uow.MembershipCards.GetByCardNumberAsync(request.Dto.CardNumber);
            if (existingCard != null) return false;

            var card = new SubscriptionCard
            {
                CardNumber = request.Dto.CardNumber,
                ValidFrom = null,
                ValidTo = null,
                PersonalTrainer = request.Dto.PersonalTrainer,
                MemberId = request.Dto.MemberId,
                IsActive = true
            };

            await _uow.MembershipCards.AddAsync(card);
            await _uow.SaveAsync();
            return true;
        }
    }
}
