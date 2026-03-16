using FlexFit.Models;

namespace FlexFit.Repositories.Interfaces
{
    public interface IFitnessObjectRepository
    {
        Task<FitnessObject> GetByIdAsync(int id);
        Task<IEnumerable<FitnessObject>> GetAllAsync();
        Task AddAsync(FitnessObject obj);
        Task UpdateAsync(FitnessObject obj);
        Task DeleteAsync(FitnessObject obj);
    }
}