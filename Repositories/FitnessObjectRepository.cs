using FlexFit.Data;
using FlexFit.Models;
using FlexFit.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlexFit.Repositories
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

        public async Task<IEnumerable<FitnessObject>> GetAllAsync() =>
            await _context.FitnessObjects.Include(f => f.Resources).ToListAsync();

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