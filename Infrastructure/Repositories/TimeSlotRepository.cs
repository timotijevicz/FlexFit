using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexFit.Application.DTOs;
using FlexFit.Infrastructure.Data;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.Repositories.Interfaces;
using FlexFit.Domain.MongoModels.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlexFit.Infrastructure.Repositories
{
    public class TimeSlotRepository : ITimeSlotRepository
    {
        private readonly AppDbContext _context;
        private readonly ReservationLogRepository _mongoRepo;

        public TimeSlotRepository(AppDbContext context, ReservationLogRepository mongoRepo)
        {
            _context = context;
            _mongoRepo = mongoRepo;
        }

        public async Task<IEnumerable<TimeSlotResultDto>> GetTimeSlotsAsync(int resourceId)
        {
            var dateToQuery = DateTime.UtcNow.Date.AddDays(1);
            await EnsureTimeSlotsAsync(resourceId, dateToQuery);

            var resource = await _context.Resources.FindAsync(resourceId);
            int capacity = (resource != null && resource.Type == FlexFit.Domain.Models.ResourceType.GrupnaSala) ? 10 : 5;

            var slots = await _context.TimeSlots
                .Where(ts => ts.ResourceId == resourceId && ts.StartTime >= dateToQuery && ts.StartTime < dateToQuery.AddDays(1))
                .OrderBy(ts => ts.StartTime)
                .ToListAsync();

            var allReservations = await _mongoRepo.GetByResourceIdAsync(resourceId);
            var reservations = allReservations
                .Where(r => r.Status != "NoShow" && r.Status != "Canceled" && r.EndTime > dateToQuery)
                .ToList();

            return slots.Select(ts => new TimeSlotResultDto
            {
                Id = ts.Id,
                ResourceId = ts.ResourceId,
                StartTime = ts.StartTime,
                EndTime = ts.EndTime,
                AvailableSpots = capacity - reservations.Count(r => 
                    (ts.StartTime >= r.StartTime && ts.StartTime < r.EndTime) || 
                    (ts.EndTime > r.StartTime && ts.EndTime <= r.EndTime) || 
                    (ts.StartTime <= r.StartTime && ts.EndTime >= r.EndTime)
                )
            });
        }

        private async Task EnsureTimeSlotsAsync(int resourceId, DateTime date)
        {
            try 
            {
                var resource = await _context.Resources.Include(r => r.FitnessObject).FirstOrDefaultAsync(r => r.Id == resourceId);
                if (resource == null || string.IsNullOrWhiteSpace(resource.FitnessObject?.WorkingHours)) return;

                var parts = resource.FitnessObject.WorkingHours.Split('-');
                if (parts.Length != 2) return;
                
                if (!TryParseTime(parts[0], out var openTime) || !TryParseTime(parts[1], out var closeTime)) return;

                var dateDate = date.Date;
                
                var existingStartTimes = await _context.TimeSlots
                    .Where(ts => ts.ResourceId == resourceId && ts.StartTime >= dateDate && ts.StartTime < dateDate.AddDays(1))
                    .Select(ts => ts.StartTime)
                    .ToListAsync();

                var hashSet = new HashSet<DateTime>(existingStartTimes);

                var slots = new List<TimeSlot>();
                var current = dateDate + openTime;
                var end = dateDate + closeTime;

                if (end < current) end = end.AddDays(1);

                while (current < end)
                {
                    var next = current.AddMinutes(30);
                    if (next > end) break;
                    
                    if (!hashSet.Contains(current))
                    {
                        slots.Add(new TimeSlot {
                            ResourceId = resourceId,
                            StartTime = current,
                            EndTime = next
                        });
                    }
                    current = next;
                }

                if (slots.Any())
                {
                    await _context.TimeSlots.AddRangeAsync(slots);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
            }
        }

        private bool TryParseTime(string input, out TimeSpan timeSpan)
        {
            input = input.Trim().ToLower().Replace("h", "").Replace("Ä", "");
            if (TimeSpan.TryParse(input, out timeSpan)) 
            {
                if (!input.Contains(":") && int.TryParse(input, out int hours))
                {
                    timeSpan = TimeSpan.FromHours(hours);
                    return true;
                }
                return true;
            }
            
            if (int.TryParse(input, out int hoursFallback))
            {
                timeSpan = TimeSpan.FromHours(hoursFallback);
                return true;
            }

            timeSpan = TimeSpan.Zero;
            return false;
        }

        public async Task<int> CreateTimeSlotAsync(TimeSlotDto dto)
        {
            if (dto.StartTime >= dto.EndTime)
                throw new ArgumentException("Pocetno vreme mora biti pre krajnjeg.");

            var timeSlot = new TimeSlot
            {
                ResourceId = dto.ResourceId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            await _context.TimeSlots.AddAsync(timeSlot);
            await _context.SaveChangesAsync();

            return timeSlot.Id;
        }

        public async Task<bool> DeleteTimeSlotAsync(int id)
        {
            var timeSlot = await _context.TimeSlots.FindAsync(id);
            if (timeSlot == null)
                return false;

            _context.TimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
