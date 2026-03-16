using FlexFit.Models;

namespace FlexFit.Repositories.Interfaces
{
    public interface IPenaltyCardRepository
    {
        Task<PenaltyCard> GetByIdAsync(int id);
        Task<bool> HasRecentPenaltyAsync(int memberId, int hours); 
        Task<IEnumerable<PenaltyCard>> GetAllAsync();
        Task AddAsync(PenaltyCard penaltyCard);
        Task UpdateAsync(PenaltyCard penaltyCard);
        Task DeleteAsync(PenaltyCard penaltyCard);
    }
}