using FlexFit.Application.Commands;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using FlexFit.Domain.MongoModels.Models;
using FlexFit.Domain.MongoModels.Repositories;
using MediatR;

namespace FlexFit.Application.Handlers
{
    public class ExtendMembershipCommandHandler : IRequestHandler<ExtendMembershipCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ExtendMembershipCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ExtendMembershipCommand request, CancellationToken cancellationToken)
        {
            var card = await _uow.MembershipCards.GetByCardNumberAsync(request.CardNumber);

            if (card == null || !(card is SubscriptionCard subCard))
            {
                return false;
            }

            subCard.ValidFrom = DateTime.UtcNow;
            subCard.ValidTo = DateTime.UtcNow.AddDays(30);
            subCard.IsActive = true;

            await _uow.MembershipCards.UpdateAsync(subCard);
            await _uow.SaveAsync();

            await _uow.MembershipLogs.AddAsync(new MembershipLog
            {
                CardNumber = subCard.CardNumber,
                Action = $"Extension for Member {subCard.MemberId}",
                NewExpiryDate = subCard.ValidTo.Value,
                Timestamp = DateTime.UtcNow
            });

            return true;
        }
    }
}
