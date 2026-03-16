using FlexFit.Data;
using FlexFit.Models;
using FlexFit.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlexFit.Repositoires
{
    public class PenaltyPointRepository : IPenaltyPointRepository
    {
        private readonly AppDbContext _context;

        public PenaltyPointRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PenaltyPoint> GetByIdAsync(int id) =>
            await _context.PenaltyPoints
                .Include(p => p.Member)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<PenaltyPoint>> GetAllAsync() =>
            await _context.PenaltyPoints.Include(p => p.Member).ToListAsync();

        public async Task AddAsync(PenaltyPoint point) => await _context.PenaltyPoints.AddAsync(point);

        public Task UpdateAsync(PenaltyPoint point)
        {
            _context.PenaltyPoints.Update(point);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(PenaltyPoint point)
        {
            _context.PenaltyPoints.Remove(point);
            return Task.CompletedTask;
        }
    }
}
