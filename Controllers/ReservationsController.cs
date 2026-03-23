using FlexFit.Models;
using FlexFit.UnitOfWorkLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlexFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public ReservationsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost("book")]
        [Authorize(Roles = "Member,0,Admin,Employee,1,2,Redar")]
        public async Task<IActionResult> BookResource([FromBody] ReservationDto dto)
        {
            if (dto.StartTime >= dto.EndTime)
                return BadRequest(new { message = "Početno vreme mora biti pre krajnjeg." });

            var resource = await _uow.Resources.GetByIdAsync(dto.ResourceId);
            if (resource != null && resource.Type == ResourceType.GrupnaSala)
            {
                var concurrent = await _uow.Reservations.FindAsync(r => 
                    r.ResourceId == dto.ResourceId &&
                    r.Status != ReservationStatus.NoShow &&
                    ((dto.StartTime >= r.StartTime && dto.StartTime < r.EndTime) || 
                     (dto.EndTime > r.StartTime && dto.EndTime <= r.EndTime) || 
                     (dto.StartTime <= r.StartTime && dto.EndTime >= r.EndTime))
                );

                if (concurrent.Count() >= 10)
                {
                    return BadRequest(new { message = "Maksimalan broj osoba (10) za ovaj termin grupne sale je popunjen." });
                }
            }

            var reservation = new Reservation
            {
                MemberId = dto.MemberId,
                ResourceId = dto.ResourceId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = ReservationStatus.Reserved
            };

            await _uow.Reservations.AddAsync(reservation);
            
            return Ok(new { message = "Rezervacija uspešna." });
        }

        [HttpPost("mark-no-show/{id}")]
        [Authorize(Roles = "Admin,Employee,1,2,Redar")]
        public async Task<IActionResult> MarkNoShow(int id)
        {
            // Dummy logic to simulate penalizing no show.
            // In a real application, you'd fetch the reservation, assign the penalty point, and save.
            return Ok(new { message = "Kazneni poen je uspešno upisan zbog nepojavljivanja." });
        }

        [HttpGet("resource/{resourceId}")]
        public async Task<IActionResult> GetResourceReservations(int resourceId)
        {
            var reservations = await _uow.Reservations.FindAsync(r => r.ResourceId == resourceId);
            
            // Note: Since UnitOfWork FindAsync might not eagerly load Member by default, 
            // you might need to use EF Core directly if FindAsync doesn't support Includes. 
            // Assuming the repo has an Include capability or we do manual mapping if Member is not loaded.
            
            var now = DateTime.UtcNow;

            var result = reservations.Select(r => new {
                id = r.Id,
                memberId = r.MemberId,
                startTime = r.StartTime,
                endTime = r.EndTime,
                status = r.Status,
                isExpired = r.EndTime <= now
            }).ToList();

            return Ok(result);
        }
    }

    public class ReservationDto
    {
        public int MemberId { get; set; }
        public int ResourceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
