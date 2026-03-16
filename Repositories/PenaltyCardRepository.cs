using FlexFit.Data;
using FlexFit.Models;
using FlexFit.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlexFit.Repositories
{
    public class PenaltyCardRepository : IPenaltyCardRepository
    {
        private readonly AppDbContext _context;

        public PenaltyCardRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> HasRecentPenaltyAsync(int memberId, int hours)
        {
            var threshold = DateTime.Now.AddHours(-hours);
            return await _context.PenaltyCards
                .AnyAsync(p => p.MemberId == memberId && p.Date >= threshold);
        }
        public async Task<PenaltyCard> GetByIdAsync(int id) =>
            await _context.PenaltyCards
                .Include(p => p.Member)
                .Include(p => p.FitnessObject)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<PenaltyCard>> GetAllAsync() =>
            await _context.PenaltyCards
                .Include(p => p.Member)
                .Include(p => p.FitnessObject)
                .ToListAsync();

        public async Task AddAsync(PenaltyCard penaltyCard)
        {
            await _context.PenaltyCards.AddAsync(penaltyCard);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PenaltyCard penaltyCard)
        {
            _context.PenaltyCards.Update(penaltyCard);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PenaltyCard penaltyCard)
        {
            _context.PenaltyCards.Remove(penaltyCard);
            await _context.SaveChangesAsync();
        }
    }
}