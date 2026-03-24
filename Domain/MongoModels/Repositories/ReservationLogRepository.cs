using FlexFit.Infrastructure.Data;
using FlexFit.Domain.MongoModels.Models;
using MongoDB.Driver;

namespace FlexFit.Domain.MongoModels.Repositories
{
    public class ReservationLogRepository
    {
        private readonly IMongoCollection<ReservationLog> _collection;

        public ReservationLogRepository(MongoDbContext context)
        {
            _collection = context.ReservationLogs;
        }

        public async Task AddAsync(ReservationLog log)
        {
            await _collection.InsertOneAsync(log);
        }

        public async Task<List<ReservationLog>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<List<ReservationLog>> GetByResourceIdAsync(int resourceId)
        {
            return await _collection.Find(r => r.ResourceId == resourceId).ToListAsync();
        }

        public async Task<List<ReservationLog>> GetByMemberIdAsync(int memberId)
        {
            return await _collection.Find(r => r.MemberId == memberId).ToListAsync();
        }

        public async Task<ReservationLog?> GetBySqlIdAsync(int sqlId)
        {
            return await _collection.Find(r => r.SqlId == sqlId).FirstOrDefaultAsync();
        }

        public async Task<ReservationLog?> GetByIdAsync(string id)
        {
            return await _collection.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<ReservationLog>> GetExpiredReservationsAsync(DateTime now)
        {
            return await _collection.Find(r => r.Status == "Reserved" && r.EndTime < now).ToListAsync();
        }

        public async Task UpdateAsync(string id, ReservationLog log)
        {
            await _collection.ReplaceOneAsync(r => r.Id == id, log);
        }
    }
}
