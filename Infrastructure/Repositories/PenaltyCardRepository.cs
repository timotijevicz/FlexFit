using FlexFit.Domain.Models;
using FlexFit.Infrastructure.Repositories.Interfaces;
using FlexFit.Domain.MongoModels.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace FlexFit.Infrastructure.Repositories
{
    public class PenaltyCardRepository : IPenaltyCardRepository
    {
        private readonly PenaltyLogRepository _mongoRepo;

        public PenaltyCardRepository(PenaltyLogRepository mongoRepo)
        {
            _mongoRepo = mongoRepo;
        }

        public async Task<PenaltyCard> GetByIdAsync(string id)
        {
            var log = await _mongoRepo.GetByIdAsync(id);
            if (log == null) return null;

            return MapToPenaltyCard(log);
        }

        public async Task<IEnumerable<PenaltyCard>> GetAllAsync()
        {
            var logs = await _mongoRepo.GetAllAsync();
            return logs.Where(l => l.Type == "Card" || l.Type == "DailyTicket")
                       .Select(MapToPenaltyCard);
        }

        public async Task AddAsync(PenaltyCard penaltyCard)
        {
            // This method is now secondary given handlers call PenaltyLogs directly,
            // but we keep it for consistency.
            await _mongoRepo.AddAsync(new Domain.MongoModels.Models.PenaltyLog
            {
                MemberId = penaltyCard.MemberId,
                FitnessObjectId = penaltyCard.FitnessObjectId,
                Reason = penaltyCard.Reason,
                Price = (double?)penaltyCard.Price,
                Type = "DailyTicket",
                Timestamp = DateTime.UtcNow,
                IsPaid = penaltyCard.IsPaid,
                IsCanceled = penaltyCard.IsCanceled,
                CancelReason = penaltyCard.CancelReason
            });
        }

        public async Task<bool> HasRecentPenaltyAsync(int memberId, int hours)
        {
            var threshold = DateTime.UtcNow.AddHours(-hours);
            var logs = await _mongoRepo.GetByMemberIdAsync(memberId);
            return logs.Any(p => p.Timestamp >= threshold && (p.Type == "Card" || p.Type == "DailyTicket"));
        }

        public async Task UpdateAsync(PenaltyCard penaltyCard)
        {
            var log = await _mongoRepo.GetByIdAsync(penaltyCard.Id);
            if (log != null)
            {
                log.Reason = penaltyCard.Reason;
                log.Price = (double?)penaltyCard.Price;
                log.Timestamp = penaltyCard.Date;
                log.IsPaid = penaltyCard.IsPaid;
                log.IsCanceled = penaltyCard.IsCanceled;
                log.CancelReason = penaltyCard.CancelReason;
                await _mongoRepo.UpdateAsync(log.Id!, log);
            }
        }

        public async Task DeleteAsync(PenaltyCard penaltyCard)
        {
            var log = await _mongoRepo.GetByIdAsync(penaltyCard.Id);
            if (log != null)
            {
                log.Reason = "[DELETED] " + log.Reason;
                await _mongoRepo.UpdateAsync(log.Id!, log);
            }
        }

        private PenaltyCard MapToPenaltyCard(Domain.MongoModels.Models.PenaltyLog log)
        {
            return new PenaltyCard
            {
                Id = log.Id ?? string.Empty,
                MemberId = log.MemberId,
                FitnessObjectId = log.FitnessObjectId ?? 0,
                Date = log.Timestamp,
                Price = (decimal)(log.Price ?? 0),
                Reason = log.Reason,
                IsPaid = log.IsPaid,
                IsCanceled = log.IsCanceled,
                CancelReason = log.CancelReason
            };
        }
    }
}