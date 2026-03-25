using FlexFit.Infrastructure.Data;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlexFit.Infrastructure.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly AppDbContext _context;
        private readonly IMemberGraphRepository _graphRepo;

        public ResourceRepository(AppDbContext context, IMemberGraphRepository graphRepo)
        {
            _context = context;
            _graphRepo = graphRepo;
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

        public async Task<Resource> CreateResourceAsync(FlexFit.Application.DTOs.CreateResourceDto dto)
        {
            var resource = new Resource
            {
                Type = dto.Type,
                Status = dto.Status,
                Floor = dto.Floor,
                IsPremium = dto.IsPremium,
                PremiumFee = dto.PremiumFee,
                FitnessObjectId = dto.FitnessObjectId
            };

            await _context.Resources.AddAsync(resource);
            await _context.SaveChangesAsync();

            try {
                await _graphRepo.LinkResourceToGymAsync(resource.Id, resource.FitnessObjectId, resource.Type.ToString(), "Gym #" + resource.FitnessObjectId);
            } catch (Exception ex) {
                Console.WriteLine($"[ResourceRepository] Neo4j Sync Error: {ex.Message}");
            }
            
            return resource;
        }
    }
}