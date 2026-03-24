using FlexFit.Domain.Models;
using FlexFit.Domain.MongoModels.Models;

namespace FlexFit.Infrastructure.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        Task<ReservationLog?> GetByIdAsync(string id);
        Task<IEnumerable<ReservationLog>> GetByMemberIdAsync(int memberId);
        Task<IEnumerable<ReservationLog>> GetExpiredReservationsAsync(DateTime now);
        Task<IEnumerable<ReservationLog>> GetAllAsync();
        Task AddAsync(ReservationLog log);
        Task UpdateAsync(ReservationLog reservation);
        Task DeleteAsync(ReservationLog reservation);
        Task<(bool isSuccess, string message)> BookResourceAsync(FlexFit.Application.DTOs.ReservationDto dto);
    }
}