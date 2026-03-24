using FlexFit.Application.Commands;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using FlexFit.Domain.Interfaces.Repositories;
using MediatR;

namespace FlexFit.Application.Handlers
{
    public class SellDailyTicketCommandHandler : IRequestHandler<SellDailyTicketCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public SellDailyTicketCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(SellDailyTicketCommand request, CancellationToken cancellationToken)
        {
            var card = await _uow.MembershipCards.GetByCardNumberAsync(request.Dto.CardNumber);

            if (card == null || !(card is DailyCard dailyCard))
            {
                return false;
            }

            if (dailyCard.IsActive)
            {
                return false; 
            }

            dailyCard.PurchaseDate = DateTime.UtcNow;
            dailyCard.IsActive = true;

            await _uow.MembershipCards.UpdateAsync(dailyCard);

            if (dailyCard.MemberId.HasValue)
            {
                await _uow.MemberGraph.AssignCardToMemberAsync(
                    dailyCard.MemberId.Value.ToString(), 
                    dailyCard.CardNumber, 
                    "Daily Ticket");
            }

            await _uow.SaveAsync();

            return true;
        }
    }
}
