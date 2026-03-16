using FlexFit.Models;

namespace FlexFit.Repositories.Interfaces
{
    public interface IMembershipCardRepository
    {
        Task<MembershipCard> GetByIdAsync(int id);
        Task<MembershipCard> GetByCardNumberAsync(string cardNumber); 
        Task<IEnumerable<MembershipCard>> GetAllAsync();
        Task AddAsync(MembershipCard card);
        Task UpdateAsync(MembershipCard card);
        Task DeleteAsync(MembershipCard card);
    }
}