using MediatR;
using FlexFit.UnitOfWorkLayer;
using FlexFit.Models;
using FlexFit.MongoModels.Models; // Mongo modeli
using FlexFit.MongoModels.Repositories; // Mongo repozitorijumi

namespace FlexFit.Application.Handlers
{
    public class ProcessEntryHandler : IRequestHandler<ProcessEntryCommand, EntryResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly EntryLogRepository _mongoRepo;

        public ProcessEntryHandler(IUnitOfWork uow, EntryLogRepository mongoRepo)
        {
            _uow = uow;
            _mongoRepo = mongoRepo;
        }

        public async Task<EntryResponse> Handle(ProcessEntryCommand request, CancellationToken ct)
        {
            // 1. SQL PROVERA
            var card = await _uow.MembershipCards.GetByCardNumberAsync(request.ScannedCardNumber);

            if (card == null)
            {
                return new EntryResponse(false, "Kartica ne postoji.");
            }

            bool isAccessGranted = false;
            var now = DateTime.UtcNow;

            // Logika provere (Subscription ili Daily)
            if (card is SubscriptionCard sub)
                isAccessGranted = sub.ValidTo >= now && sub.ValidFrom <= now;
            else if (card is DailyCard daily)
                isAccessGranted = daily.PurchaseDate.Date == now.Date;

            // 2. KAZNE (SQL LOGIKA)
            if (!isAccessGranted)
            {
                bool hasRecentPenalty = await _uow.PenaltyCards.HasRecentPenaltyAsync(card.MemberId, request.FitnessObjectId);
                if (!hasRecentPenalty)
                {
                    await _uow.PenaltyCards.AddAsync(new PenaltyCard
                    {
                        MemberId = card.MemberId,
                        FitnessObjectId = request.FitnessObjectId,
                        Date = now,
                        Reason = "Nevalidna kartica",
                        Price = 500
                    });
                }
            }

            // 3. MONGO LOGOVANJE (Ovo koristi tvoj repository)
            var log = new EntryLog
            {
                MemberId = card.MemberId,
                FitnessObjectId = request.FitnessObjectId,
                Time = now,
                CardStatus = isAccessGranted ? "Validna" : "Istekla/Nevalidna",
                Incident = !isAccessGranted // Ako nije ušao, to je incident za tvoj model
            };

            await _mongoRepo.AddAsync(log);

            // 4. SAVE
            await _uow.SaveAsync(); // Čuva SQL izmene

            return isAccessGranted
                ? new EntryResponse(true, "Dobrodošli!")
                : new EntryResponse(false, "Pristup odbijen, izdata kazna.");
        }
    }
}