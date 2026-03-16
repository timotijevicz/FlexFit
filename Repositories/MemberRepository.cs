using FlexFit.Data;
using FlexFit.Models;
using FlexFit.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlexFit.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly AppDbContext _context;

        public MemberRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Member> GetByEmailAsync(string email) =>
    await _context.Members
        .Include(m => m.Reservations)
        .Include(m => m.PenaltyPointHistory)
        .FirstOrDefaultAsync(m => m.Email == email);
        public async Task<Member> GetByIdAsync(int id) =>
            await _context.Members
                .Include(m => m.Reservations)
                .Include(m => m.PenaltyPointHistory)
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<IEnumerable<Member>> GetAllAsync() =>
            await _context.Members
                .Include(m => m.Reservations)
                .Include(m => m.PenaltyPointHistory)
                .ToListAsync();

        public async Task AddAsync(Member member)
        {
            await _context.Members.AddAsync(member);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Member member)
        {
            _context.Members.Update(member);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Member member)
        {
            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
        }
    }
}