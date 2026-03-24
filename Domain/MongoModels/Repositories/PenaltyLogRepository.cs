using MongoDB.Driver;
using FlexFit.Domain.MongoModels.Models;
using FlexFit.Infrastructure.Data;
using System.Linq;

namespace FlexFit.Domain.MongoModels.Repositories
{
    public class PenaltyLogRepository
    {
        private readonly IMongoCollection<PenaltyLog> _collection;

        public PenaltyLogRepository(MongoDbContext context)
        {
            _collection = context.PenaltyLogs;
        }

        public async Task<PenaltyLog?> GetByIdAsync(string id)
        {
            return await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddAsync(PenaltyLog log)
        {
            Console.WriteLine($"[PenaltyLogRepo] Inserting to MongoDB: MemberId={log.MemberId}, Type={log.Type}, Reason={log.Reason}");
            await _collection.InsertOneAsync(log);
        }

        public async Task<List<PenaltyLog>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<List<PenaltyLog>> GetByMemberIdAsync(int memberId)
        {
            return await _collection.Find(p => p.MemberId == memberId).ToListAsync();
        }

        public async Task<PenaltyLog?> GetBySqlIdAsync(int sqlId, string type)
        {
            return await _collection.Find(p => p.SqlId == sqlId && p.Type == type).FirstOrDefaultAsync();
        }

        public async Task<PenaltyLog?> GetBySqlIdAnyTypeAsync(int sqlId, string[] types)
        {
            return await _collection.Find(p => p.SqlId == sqlId && types.Contains(p.Type)).FirstOrDefaultAsync();
        }

        public async Task<List<PenaltyLog>> GetByTypeAsync(string type)
        {
            return await _collection.Find(p => p.Type == type).ToListAsync();
        }

        public async Task UpdateAsync(string id, PenaltyLog log)
        {
            await _collection.ReplaceOneAsync(p => p.Id == id, log);
        }
    }
}
