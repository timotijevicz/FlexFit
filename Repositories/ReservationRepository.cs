using FlexFit.Data;
using FlexFit.Models;
using FlexFit.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlexFit.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;

        public ReservationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Reservation> GetByIdAsync(int id) =>
            await _context.Reservations
                .Include(r => r.Member)
                .Include(r => r.Resource)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<IEnumerable<Reservation>> GetAllAsync() =>
            await _context.Reservations
                .Include(r => r.Member)
                .Include(r => r.Resource)
                .ToListAsync();

        public async Task AddAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Reservation reservation)
        {
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }
    }
}