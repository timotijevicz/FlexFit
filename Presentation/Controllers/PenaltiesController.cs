using FlexFit.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using FlexFit.Domain.Models;

namespace FlexFit.Presentation.Controllers
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
            var logs = await _uow.PenaltyLogs.GetByTypeAsync("DailyTicket");
            return Ok(logs);
        }

        [HttpGet("points")]
        public async Task<IActionResult> GetAllPoints()
        {
            var logs = await _uow.PenaltyLogs.GetByTypeAsync("Point");
            return Ok(logs);
        }

        [HttpPost("cards")]
        public async Task<IActionResult> CreateCard([FromBody] CreatePenaltyCardCommand command)
        {
            var success = await _mediator.Send(command);
            if (!success) return BadRequest(new { message = "Clan je vec dobio kaznenu kartu u ovom objektu u poslednjih 12h." });
            return Ok(new { message = "Kaznena karta uspesno izdata." });
        }

        [HttpPost("points")]
        public async Task<IActionResult> CreatePoint([FromBody] CreatePenaltyPointCommand command)
        {
            var success = await _mediator.Send(command);
            if (!success) return BadRequest(new { message = "Greska pri izdavanju kaznenog poena." });
            return Ok(new { message = "Kazneni poen uspesno dodat." });
        }

        public class CancelDto { public string Type { get; set; } public string Reason { get; set; } }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(string id, [FromBody] CancelDto dto)
        {
            var success = await _mediator.Send(new CancelPenaltyCommand(id, dto.Type, dto.Reason));
            if (!success) return NotFound(new { message = "Kazna nije pronaÄ‘ena ili je veÄ‡ stornirana." });
            return Ok(new { message = "Uspesno stornirano uz napomenu." });
        }

        [HttpPost("{id}/pay")]
        public async Task<IActionResult> Pay(string id)
        {
            var success = await _mediator.Send(new PayPenaltyCommand(id));
            if (!success) return BadRequest(new { message = "Kazna nije pronadjena, vec je placena ili je stornirana." });
            return Ok(new { message = "Kazna je uspesno placena." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, [FromQuery] string type)
        {
            var success = await _mediator.Send(new DeletePenaltyCommand(id, type));
            if (!success) return NotFound(new { message = "Kazna nije pronadjena." });
            return Ok(new { message = "Uspesno obrisano." });
        }
    }
}
