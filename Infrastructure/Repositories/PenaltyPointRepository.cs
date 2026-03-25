using FlexFit.Domain.Models;
using FlexFit.Infrastructure.Repositories.Interfaces;
using FlexFit.Domain.MongoModels.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace FlexFit.Infrastructure.Repositories
{
    public class PenaltyPointRepository : IPenaltyPointRepository
    {
        private readonly PenaltyLogRepository _mongoRepo;

        public PenaltyPointRepository(PenaltyLogRepository mongoRepo)
        {
            _mongoRepo = mongoRepo;
        }

        public async Task<PenaltyPoint> GetByIdAsync(string id)
        {
            var log = await _mongoRepo.GetByIdAsync(id);
            if (log == null) return null;

            return MapToPenaltyPoint(log);
        }

        public async Task<IEnumerable<PenaltyPoint>> GetAllAsync()
        {
            var logs = await _mongoRepo.GetByTypeAsync("Point");
            return logs.Select(MapToPenaltyPoint);
        }

        public async Task AddAsync(PenaltyPoint point)
        {
            await _mongoRepo.AddAsync(new Domain.MongoModels.Models.PenaltyLog
            {
                MemberId = point.MemberId,
                Reason = point.Description,
                Type = "Point",
                Date = DateTime.UtcNow,
                IsCanceled = point.IsCanceled,
                CancelReason = point.CancelReason
            });
        }

        public async Task UpdateAsync(PenaltyPoint point)
        {
            var log = await _mongoRepo.GetByIdAsync(point.Id);
            if (log != null)
            {
                log.Reason = point.Description;
                log.IsCanceled = point.IsCanceled;
                log.CancelReason = point.CancelReason;
                await _mongoRepo.UpdateAsync(log.Id!, log);
            }
        }

        public async Task DeleteAsync(PenaltyPoint point)
        {
            var log = await _mongoRepo.GetByIdAsync(point.Id);
            if (log != null)
            {
                log.Reason = "[DELETED] " + log.Reason;
                await _mongoRepo.UpdateAsync(log.Id!, log);
            }
        }

        private PenaltyPoint MapToPenaltyPoint(Domain.MongoModels.Models.PenaltyLog log)
        {
            return new PenaltyPoint
            {
                Id = log.Id ?? string.Empty,
                MemberId = log.MemberId,
                Description = log.Reason,
                Date = log.Date,
                IsCanceled = log.IsCanceled,
                CancelReason = log.CancelReason
            };
        }
    }
}
