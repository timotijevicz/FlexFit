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
                ValidFrom = DateTime.MinValue, // Default value, will be set when member registers
                ValidTo = DateTime.MinValue, // Default value, will be set when member registers
                PersonalTrainer = request.Dto.PersonalTrainer,
                CardType = CardType.Subscription
            };

            await _uow.MembershipCards.AddAsync(card);
            await _uow.SaveAsync();
            return true;
        }
    }
}
