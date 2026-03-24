using FlexFit.Application.Commands;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using MediatR;
using BCrypt.Net;

using FlexFit.Domain.Interfaces.Repositories;

namespace FlexFit.Application.Handlers
{
    public class RegisterMemberCommandHandler : IRequestHandler<RegisterMemberCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public RegisterMemberCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(RegisterMemberCommand request, CancellationToken cancellationToken)
        {
           
            var card = await _uow.MembershipCards.GetByCardNumberAsync(request.Dto.CardNumber);

            if (card == null || card.MemberId != null || !(card is SubscriptionCard subCard)) 
            {
                return false; 
            }

        
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Dto.Password);

            var newMember = new Member
            {
                FirstName = request.Dto.FirstName,
                LastName = request.Dto.LastName,
                Address = request.Dto.Address,
                Email = request.Dto.Email,
                Password = hashedPassword,
                JMBG = request.Dto.JMBG,
                Role = Role.Member,
                PenaltyPoints = 0
            };

            await _uow.Members.AddAsync(newMember);
            await _uow.SaveAsync(); 

    
            card.MemberId = newMember.Id;
            newMember.SubscriptionCards.Add(subCard);
            
            subCard.PersonalTrainer = request.Dto.PersonalTrainer;
            subCard.IsActive = true;

            await _uow.MembershipCards.UpdateAsync(card);
            await _uow.SaveAsync();

            return true;
        }
    }
}
