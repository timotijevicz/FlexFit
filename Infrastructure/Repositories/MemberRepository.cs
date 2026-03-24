using FlexFit.Infrastructure.Data;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlexFit.Infrastructure.Repositories
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
        .FirstOrDefaultAsync(m => m.Email == email);
        public async Task<Member> GetByIdAsync(int id) =>
            await _context.Members
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<IEnumerable<Member>> GetAllAsync() =>
            await _context.Members
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