using FlexFit.Data;
using FlexFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeSlotsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TimeSlotsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{resourceId}")]
        public async Task<IActionResult> GetTimeSlots(int resourceId)
        {
            try {
                var slots = await _context.TimeSlots
                    .Where(ts => ts.ResourceId == resourceId)
                    .OrderBy(ts => ts.StartTime)
                    .ToListAsync();

                var reservations = await _context.Reservations
                    .Where(r => r.ResourceId == resourceId && r.Status != FlexFit.Models.ReservationStatus.NoShow)
                    .ToListAsync();

                var result = slots.Select(ts => new {
                    id = ts.Id,
                    resourceId = ts.ResourceId,
                    startTime = ts.StartTime,
                    endTime = ts.EndTime,
                    availableSpots = 10 - reservations.Count(r => 
                        (ts.StartTime >= r.StartTime && ts.StartTime < r.EndTime) || 
                        (ts.EndTime > r.StartTime && ts.EndTime <= r.EndTime) || 
                        (ts.StartTime <= r.StartTime && ts.EndTime >= r.EndTime)
                    )
                });

                return Ok(result);
            } catch (Exception ex) {
                return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPost]
        // [Authorize(Roles = "Admin,Employee,1,2,Redar")]
        public async Task<IActionResult> CreateTimeSlot([FromBody] TimeSlotDto dto)
        {
            try {
                if (dto.StartTime >= dto.EndTime)
                    return BadRequest("Početno vreme mora biti pre krajnjeg.");

                var timeSlot = new TimeSlot
                {
                    ResourceId = dto.ResourceId,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime
                };

                await _context.TimeSlots.AddAsync(timeSlot);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Predloženi termin uspešno dodat.", id = timeSlot.Id });
            } catch (Exception ex) {
                return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Employee,1,2,Redar")]
        public async Task<IActionResult> DeleteTimeSlot(int id)
        {
            var timeSlot = await _context.TimeSlots.FindAsync(id);
            if (timeSlot == null)
                return NotFound("Termin nije pronađen.");

            _context.TimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Termin uspešno obrisan." });
        }
    }

    public class TimeSlotDto
    {
        public int ResourceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
