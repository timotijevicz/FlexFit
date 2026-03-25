using FlexFit.Domain.Models;

namespace FlexFit.Infrastructure.Repositories.Interfaces
{
    public interface IFitnessObjectRepository
    {
        Task<FitnessObject> GetByIdAsync(int id);
        Task<IEnumerable<FitnessObject>> GetAllAsync(string searchTerm = null, string city = null);
        Task AddAsync(FitnessObject obj);
        Task UpdateAsync(FitnessObject obj);
        Task DeleteAsync(FitnessObject obj);
    }
}