using FlexFit.Data;
using FlexFit.Models;
using FlexFit.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlexFit.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly AppDbContext _context;

        public ResourceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Resource> GetByIdAsync(int id) =>
            await _context.Resources.FirstOrDefaultAsync(r => r.Id == id);

        public async Task<IEnumerable<Resource>> GetAllAsync() =>
            await _context.Resources.ToListAsync();

        public async Task AddAsync(Resource resource)
        {
            await _context.Resources.AddAsync(resource);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Resource resource)
        {
            _context.Resources.Update(resource);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Resource resource)
        {
            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();
        }
    }
}