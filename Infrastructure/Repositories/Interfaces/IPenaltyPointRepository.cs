using FlexFit.Domain.Models;

namespace FlexFit.Infrastructure.Repositories.Interfaces
{
    public interface IPenaltyPointRepository
    {
        Task<PenaltyPoint> GetByIdAsync(string id);
        Task<IEnumerable<PenaltyPoint>> GetAllAsync();
        Task AddAsync(PenaltyPoint point);
        Task UpdateAsync(PenaltyPoint point);
        Task DeleteAsync(PenaltyPoint point);
    }
}