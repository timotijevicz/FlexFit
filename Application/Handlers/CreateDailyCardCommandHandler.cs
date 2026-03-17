using FlexFit.Application.Commands;
using FlexFit.Models;
using FlexFit.UnitOfWorkLayer;
using MediatR;

namespace FlexFit.Application.Handlers
{
    public class CreateDailyCardCommandHandler : IRequestHandler<CreateDailyCardCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        public CreateDailyCardCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<bool> Handle(CreateDailyCardCommand request, CancellationToken cancellationToken)
        {
            // Provera da li kartica ve? postoji
            var existingCard = await _uow.MembershipCards.GetByCardNumberAsync(request.Dto.CardNumber);
            if (existingCard != null) return false;

            // Kreiranje nove DailyCard
            var card = new DailyCard
            {
                CardNumber = request.Dto.CardNumber,
                SerialNumber = request.Dto.SerialNumber,
                PurchaseDate = DateTime.UtcNow.Date,
                PurchaseTime = DateTime.UtcNow.TimeOfDay,
                CardType = CardType.Daily
            };

            // Dodavanje jednog ili više FitnessObject-ova (ako ih DTO sadrži)
            if (request.Dto.FitnessObjectIds != null && request.Dto.FitnessObjectIds.Any())
            {
                foreach (var objId in request.Dto.FitnessObjectIds)
                {
                    var fitnessObject = await _uow.FitnessObjects.GetByIdAsync(objId);
                    if (fitnessObject != null)
                    {
                        card.FitnessObjects.Add(fitnessObject);
                    }
                }
            }

            // ?uvanje kartice u bazi
            await _uow.MembershipCards.AddAsync(card);
            await _uow.SaveAsync();

            return true;
        }
    }
}