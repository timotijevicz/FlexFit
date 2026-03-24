using FlexFit.Domain.Models;

namespace FlexFit.Infrastructure.Repositories.Interfaces
{
    public interface IPenaltyCardRepository
    {
        Task<PenaltyCard> GetByIdAsync(string id);
        Task<bool> HasRecentPenaltyAsync(int memberId, int hours); 
        Task<IEnumerable<PenaltyCard>> GetAllAsync();
        Task AddAsync(PenaltyCard penaltyCard);
        Task UpdateAsync(PenaltyCard penaltyCard);
        Task DeleteAsync(PenaltyCard penaltyCard);
    }
}