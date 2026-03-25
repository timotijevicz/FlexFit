using FlexFit.Application.Commands;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using FlexFit.Infrastructure.Repositories.Interfaces;
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
            
            try {
                await _uow.MemberGraph.AssignCardToMemberAsync(card.MemberId.ToString(), card.CardNumber, "Subscription");
            } catch (Exception ex) {
                Console.WriteLine($"[CreateSubscriptionCardHandler] Neo4j Sync Error: {ex.Message}");
            }

            await _uow.SaveAsync();
            return true;
        }
    }
}
