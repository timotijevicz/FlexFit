using FlexFit.Infrastructure.Data;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlexFit.Infrastructure.Repositories
{
    public class FitnessObjectRepository : IFitnessObjectRepository
    {
        private readonly AppDbContext _context;

        public FitnessObjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FitnessObject> GetByIdAsync(int id) =>
            await _context.FitnessObjects
                .Include(f => f.Resources)
                .FirstOrDefaultAsync(f => f.Id == id);

        public async Task<IEnumerable<FitnessObject>> GetAllAsync(string searchTerm = null, string city = null)
        {
            var query = _context.FitnessObjects.Include(f => f.Resources).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearch = searchTerm.ToLower();
                query = query.Where(f => 
                    f.Name.ToLower().Contains(lowerSearch) || 
                    f.Address.ToLower().Contains(lowerSearch) || 
                    f.City.ToLower().Contains(lowerSearch));
            }

            if (!string.IsNullOrWhiteSpace(city) && city != "Sve")
            {
                query = query.Where(f => f.City == city);
            }

            return await query.ToListAsync();
        }

        public async Task AddAsync(FitnessObject obj)
        {
            await _context.FitnessObjects.AddAsync(obj);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FitnessObject obj)
        {
            _context.FitnessObjects.Update(obj);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(FitnessObject obj)
        {
            _context.FitnessObjects.Remove(obj);
            await _context.SaveChangesAsync();
        }
    }
}