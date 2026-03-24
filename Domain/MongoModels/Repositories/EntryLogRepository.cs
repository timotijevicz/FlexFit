using FlexFit.Infrastructure.Data;
using FlexFit.Domain.MongoModels.Models;
using MongoDB.Driver;

namespace FlexFit.Domain.MongoModels.Repositories
{
    public class EntryLogRepository
    {
        private readonly IMongoCollection<EntryLog> _collection;

        public EntryLogRepository(MongoDbContext context)
        {
            _collection = context.EntryLogs;
        }

        public async Task AddAsync(EntryLog log)
        {
            Console.WriteLine($"[EntryLogRepo] Inserting log for member {log.MemberId}...");
            await _collection.InsertOneAsync(log);
            Console.WriteLine($"[EntryLogRepo] Log inserted successfully.");
        }

        public async Task<List<EntryLog>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }
    }
}
