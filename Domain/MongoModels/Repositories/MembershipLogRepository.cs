using FlexFit.Infrastructure.Data;
using FlexFit.Domain.MongoModels.Models;
using MongoDB.Driver;

namespace FlexFit.Domain.MongoModels.Repositories
{
    public class MembershipLogRepository
    {
        private readonly IMongoCollection<MembershipLog> _collection;

        public MembershipLogRepository(MongoDbContext context)
        {
            _collection = context.MembershipLogs;
        }

        public async Task AddAsync(MembershipLog log)
        {
            await _collection.InsertOneAsync(log);
        }

        public async Task<List<MembershipLog>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }
    }
}
