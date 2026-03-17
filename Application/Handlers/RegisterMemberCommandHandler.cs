using FlexFit.Application.Commands;
using FlexFit.Models;
using FlexFit.UnitOfWorkLayer;
using MediatR;
using BCrypt.Net;

namespace FlexFit.Application.Handlers
{
    //public class RegisterMemberCommandHandler : IRequestHandler<RegisterMemberCommand, bool>
    //{
    //    private readonly IUnitOfWork _uow;

    //    public RegisterMemberCommandHandler(IUnitOfWork uow)
    //    {
    //        _uow = uow;
    //    }

    //    //public async Task<bool> Handle(RegisterMemberCommand request, CancellationToken cancellationToken)
    //    //{
    //    //    // 1. Verify Card Number exists
    //    //    var card = await _uow.MembershipCards.GetByCardNumberAsync(request.Dto.CardNumber);

    //    //    if (card == null || card.MemberId != 0 || card.CardType != CardType.Subscription) // Cannot register if card does not exist, is already assigned, or is not a subscription card
    //    //    {
    //    //        return false; 
    //    //    }

    //    //    // 2. Hash Password
    //    //    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Dto.Password);

    //    //    // 3. Create Member
    //    //    var newMember = new Member
    //    //    {
    //    //        FirstName = request.Dto.FirstName,
    //    //        LastName = request.Dto.LastName,
    //    //        Address = request.Dto.Address,
    //    //        Email = request.Dto.Email,
    //    //        Password = hashedPassword,
    //    //        JMBG = request.Dto.JMBG,
    //    //        Role = Role.Member,
    //    //        PenaltyPoints = 0
    //    //    };

    //    //    await _uow.Members.AddAsync(newMember);
    //    //    await _uow.SaveAsync(); // Save to generate Member.Id

    //    //    // 4. Assign Card to Member
    //    //    card.MemberId = newMember.Id;
    //    //    newMember.ActiveCard = card;

    //    //    if (card is SubscriptionCard subCard)
    //    //    {
    //    //        subCard.ValidFrom = DateTime.UtcNow.Date;
    //    //        subCard.ValidTo = DateTime.UtcNow.Date.AddDays(30);
    //    //    }

    //    //    await _uow.MembershipCards.UpdateAsync(card);
    //    //    await _uow.SaveAsync();

    //    //    return true;
    //    //}
    //}
}
