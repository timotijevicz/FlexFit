using FlexFit.Infrastructure.Data;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using FlexFit.Application.DTOs;
using FlexFit.Domain.Interfaces.Repositories;
using FlexFit.Domain.MongoModels.Models;
using FlexFit.Domain.MongoModels.Repositories;

namespace FlexFit.Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;
        private readonly IMemberGraphRepository _graphRepo;
        private readonly ReservationLogRepository _mongoRepo;

        public ReservationRepository(AppDbContext context, IMemberGraphRepository graphRepo, ReservationLogRepository mongoRepo)
        {
            _context = context;
            _graphRepo = graphRepo;
            _mongoRepo = mongoRepo;
        }

        public async Task<ReservationLog?> GetByIdAsync(string id) =>
            await _mongoRepo.GetByIdAsync(id);

        public async Task<IEnumerable<ReservationLog>> GetByMemberIdAsync(int memberId) =>
            await _mongoRepo.GetByMemberIdAsync(memberId);

        public async Task<IEnumerable<ReservationLog>> GetExpiredReservationsAsync(DateTime now) =>
            await _mongoRepo.GetExpiredReservationsAsync(now);

        public async Task<IEnumerable<ReservationLog>> GetAllAsync() =>
            await _mongoRepo.GetAllAsync();

        public async Task AddAsync(ReservationLog reservation)
        {
            // Original SQL write (Commented out per user request)
            /*
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
            */

            // MongoDB mirror (Logic Primary)
            await _mongoRepo.AddAsync(reservation);
        }

        public async Task UpdateAsync(ReservationLog log)
        {
            // 1. MongoDB Update
            await _mongoRepo.UpdateAsync(log.Id, log);

            // 2. Sync to PostgreSQL
            // if (log.SqlId.HasValue)
            // {
            //     var sqlRes = await _context.Reservations.FindAsync(log.SqlId.Value);
            //     if (sqlRes != null)
            //     {
            //         sqlRes.Status = Enum.Parse<ReservationStatus>(log.Status);
            //         await _context.SaveChangesAsync();
            //     }
            // }
        }

        public async Task DeleteAsync(ReservationLog log)
        {
            // 1. MongoDB Status Update
            log.Status = "Canceled";
            await _mongoRepo.UpdateAsync(log.Id, log);

            // 2. Sync to PostgreSQL (Commented out per user request)
            /*
            if (log.SqlId.HasValue)
            {
                var sqlRes = await _context.Reservations.FindAsync(log.SqlId.Value);
                if (sqlRes != null)
                {
                    _context.Reservations.Remove(sqlRes);
                    await _context.SaveChangesAsync();
                }
            }
            */
        }

        public async Task<(bool isSuccess, string message)> BookResourceAsync(ReservationDto dto)
        {
            try
            {
                if (dto.StartTime >= dto.EndTime)
                    return (false, "Pocetno vreme mora biti pre krajnjeg.");

                // Use MongoDB for checks
                var memberReservations = await _mongoRepo.GetByMemberIdAsync(dto.MemberId);
                var alreadyBooked = memberReservations?.Any(r => 
                    r.ResourceId == dto.ResourceId && 
                    r.StartTime == dto.StartTime && 
                    r.Status != "NoShow" && r.Status != "Canceled") ?? false;

                if (alreadyBooked)
                {
                    return (false, "Vec ste uspesno zakazali ovaj termin. Nije moguce zakazati isti termin vise puta.");
                }

                // Still need context to check resource capacity/type
                var resource = await _context.Resources.FindAsync(dto.ResourceId);
                int maxCapacity = (resource != null && resource.Type == ResourceType.GrupnaSala) ? 10 : 5;

                var resourceReservations = await _mongoRepo.GetByResourceIdAsync(dto.ResourceId);
                var concurrent = resourceReservations?
                    .Where(r => 
                        r.Status != "NoShow" && r.Status != "Canceled" &&
                        ((dto.StartTime >= r.StartTime && dto.StartTime < r.EndTime) || 
                         (dto.EndTime > r.StartTime && dto.EndTime <= r.EndTime) || 
                         (dto.StartTime <= r.StartTime && dto.EndTime >= r.EndTime))
                    ).ToList() ?? new List<ReservationLog>();

                if (concurrent.Count >= maxCapacity)
                {
                    return (false, $"Maksimalan broj osoba ({maxCapacity}) za ovaj termin je vec popunjen.");
                }

                try {
                    await _graphRepo.RecordReservationAsync(dto.MemberId.ToString(), dto.ResourceId);
                } catch (Exception ex) {
                    Console.WriteLine($"Neo4j Sync Error: {ex.Message}");
                }

                // Write to MongoDB (Sole Primary)
                await _mongoRepo.AddAsync(new ReservationLog
                {
                    MemberId = dto.MemberId,
                    ResourceId = dto.ResourceId,
                    SqlId = 0, // No longer using SQL ID
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    Status = "Reserved",
                    LogTime = DateTime.UtcNow
                });
                
                return (true, "Rezervacija uspesna.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BookResourceAsync: {ex.Message}\n{ex.StackTrace}");
                return (false, $"Greska: {ex.Message}");
            }
        }
    }
}
