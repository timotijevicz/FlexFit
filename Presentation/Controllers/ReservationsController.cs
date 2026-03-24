using FlexFit.Domain.Models;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using FlexFit.Application.DTOs;
using FlexFit.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlexFit.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMediator _mediator;

        public ReservationsController(IUnitOfWork uow, IMediator mediator)
        {
            _uow = uow;
            _mediator = mediator;
        }

        [HttpPost("book")]
        [Authorize(Roles = "Member,Admin,Employee")]
        public async Task<IActionResult> BookResource([FromBody] ReservationDto dto)
        {
            var result = await _uow.Reservations.BookResourceAsync(dto);

            if (!result.isSuccess)
            {
                return BadRequest(new { message = result.message });
            }
            
            return Ok(new { message = result.message });
        }

        [HttpPost("mark-no-show/{id}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> MarkNoShow(string id)
        {
            var log = await _uow.Reservations.GetByIdAsync(id);
            if (log == null) return NotFound(new { message = "Rezervacija nije pronadjena." });

            log.Status = "NoShow";
            await _uow.Reservations.UpdateAsync(log);
            await _uow.SaveAsync();

            var result = await _mediator.Send(new ProcessNoShowPenaltyCommand(id));
            if (!result) return BadRequest(new { message = "Greska pri dodeljivanju kaznenog poena." });

            return Ok(new { message = "Rezervacija oznacena kao 'NoShow' i kazneni poen je uspesno upisan." });
        }

        [HttpDelete("cancel/{id}")]
        [Authorize(Roles = "Member,Admin,Employee")]
        public async Task<IActionResult> CancelReservation(string id)
        {
            var log = await _uow.Reservations.GetByIdAsync(id);
            if (log == null)
            {
                return NotFound(new { message = "Rezervacija nije pronadjena." });
            }

            await _uow.Reservations.DeleteAsync(log);

            return Ok(new { message = "Rezervacija je uspesno otkazana." });
        }

        [HttpGet("resource/{resourceId}")]
        public async Task<IActionResult> GetResourceReservations(int resourceId)
        {
            var logs = await _uow.ReservationLogs.GetByResourceIdAsync(resourceId);
            return Ok(logs);
        }
    }
}
