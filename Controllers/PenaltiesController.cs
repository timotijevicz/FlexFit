using FlexFit.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FlexFit.UnitOfWorkLayer;
using FlexFit.Models;

namespace FlexFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PenaltiesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _uow;

        public PenaltiesController(IMediator mediator, IUnitOfWork uow)
        {
            _mediator = mediator;
            _uow = uow;
        }

        [HttpGet("cards")]
        public async Task<IActionResult> GetAllCards()
        {
            var cards = await _uow.PenaltyCards.GetAllAsync();
            return Ok(cards);
        }

        [HttpGet("points")]
        public async Task<IActionResult> GetAllPoints()
        {
            var points = await _uow.PenaltyPoints.GetAllAsync();
            return Ok(points);
        }

        [HttpPost("cards")]
        public async Task<IActionResult> CreateCard([FromBody] CreatePenaltyCardCommand command)
        {
            var success = await _mediator.Send(command);
            if (!success) return BadRequest(new { message = "Član je već dobio kaznenu kartu u ovom objektu u poslednjih 12h." });
            return Ok(new { message = "Kaznena karta uspešno izdata." });
        }

        [HttpPost("points")]
        public async Task<IActionResult> CreatePoint([FromBody] CreatePenaltyPointCommand command)
        {
            var success = await _mediator.Send(command);
            if (!success) return BadRequest(new { message = "Greška pri izdavanju kaznenog poena." });
            return Ok(new { message = "Kazneni poen uspešno dodat." });
        }

        public class CancelDto { public string Type { get; set; } public string Reason { get; set; } }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelDto dto)
        {
            var success = await _mediator.Send(new CancelPenaltyCommand(id, dto.Type, dto.Reason));
            if (!success) return NotFound(new { message = "Kazna nije pronađena ili je već stornirana." });
            return Ok(new { message = "Uspešno stornirano uz napomenu." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] string type)
        {
            var success = await _mediator.Send(new DeletePenaltyCommand(id, type));
            if (!success) return NotFound(new { message = "Kazna nije pronađena." });
            return Ok(new { message = "Uspešno obrisano." });
        }
    }
}
