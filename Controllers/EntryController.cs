using MediatR;
using Microsoft.AspNetCore.Mvc;
using FlexFit.Application.Handlers;

namespace FlexFit.Controllers
{
    [ApiController]
    [Route("api/entry")]
    public class EntryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EntryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("scan")]
        public async Task<IActionResult> Scan([FromBody] string cardNumber, [FromQuery] int fitnessId)
        {
            var response = await _mediator.Send(new ProcessEntryCommand(cardNumber, fitnessId));

            if (response.IsSuccess)
                return Ok(response);

            return BadRequest(response);
        }
    }
}