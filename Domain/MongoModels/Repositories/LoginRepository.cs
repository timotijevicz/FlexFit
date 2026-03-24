using FlexFit.Infrastructure.Data;
using FlexFit.Domain.MongoModels.Models;
using MongoDB.Driver;

namespace FlexFit.Domain.MongoModels.Repositories
{
    public class LoginRepository
    {
        private readonly IMongoCollection<Login> _collection;

        public LoginRepository(MongoDbContext context)
        {
            _collection = context.Login;
        }

        public async Task AddAsync(Login log)
        {
            await _collection.InsertOneAsync(log);
        }

        public async Task<List<Login>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<List<Login>> GetByUserIdAsync(string userId)
        {
            return await _collection.Find(l => l.UserId == userId).ToListAsync();
        }

        public async Task<List<Login>> GetByEmailAsync(string email)
        {
            return await _collection.Find(l => l.Email == email).ToListAsync();
        }

        public async Task<List<Login>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _collection.Find(l => l.Time >= from && l.Time <= to).ToListAsync();
        }

        public async Task<Login?> GetLastLoginAsync(string userId)
        {
            return await _collection
                         .Find(l => l.UserId == userId)
                         .SortByDescending(l => l.Time)
                         .FirstOrDefaultAsync();
        }
    }
}